using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Airbnb.BookingService.Infrastructure;
using Airbnb.BookingService.Domain;
using System.Text.Json;

namespace Airbnb.BookingService.Infrastructure;

public class PaymentConsumer(
    ILogger<PaymentConsumer> logger,
    IConsumer<string, string> consumer,
    IServiceScopeFactory scopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Debezium topic cho bảng OutboxEvents
        const string topic = "airbnb.public.OutboxEvents";
        consumer.Subscribe(topic);
        
        logger.LogInformation("Subscribed to Outbox CDC events: {topic}", topic);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result = consumer.Consume(stoppingToken);
                if (result == null) continue;

                using var doc = JsonDocument.Parse(result.Message.Value);
                var payload = doc.RootElement.GetProperty("payload");
                var op = payload.GetProperty("op").GetString();

                // Chỉ quan tâm đến 'c' (create) vì đó là lúc event mới được lưu vào Outbox
                if (op != "c") continue;

                var after = payload.GetProperty("after");
                var eventId = Guid.Parse(after.GetProperty("Id").GetString()!);
                var eventType = after.GetProperty("EventType").GetString();
                var eventPayloadJson = after.GetProperty("Payload").GetString();

                if (eventType != "PaymentSuccess") continue;

                using var scope = scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<BookingDbContext>();

                // Staff-level SOP: Idempotency Check in Transaction
                await using var transaction = await db.Database.BeginTransactionAsync(stoppingToken);
                try
                {
                    bool alreadyProcessed = await db.ProcessedEvents.AnyAsync(e => e.EventId == eventId, stoppingToken);
                    if (alreadyProcessed)
                    {
                        logger.LogWarning("Event {id} already processed. Skipping.", eventId);
                        await transaction.RollbackAsync(stoppingToken);
                        continue;
                    }

                    var evtData = JsonDocument.Parse(eventPayloadJson!).RootElement;
                    var bookingId = Guid.Parse(evtData.GetProperty("BookingId").GetString()!);

                    var booking = await db.Bookings.FindAsync([bookingId], stoppingToken);
                    if (booking != null)
                    {
                        booking.Confirm();
                        
                        // Đánh dấu đã xử lý
                        db.ProcessedEvents.Add(new ProcessedEvent 
                        { 
                            EventId = eventId, 
                            EventType = eventType! 
                        });

                        await db.SaveChangesAsync(stoppingToken);
                        await transaction.CommitAsync(stoppingToken);
                        
                        logger.LogInformation("Booking {id} CONFIRMED (Idempotent)", bookingId);
                    }
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(stoppingToken);
                    logger.LogError(ex, "Failed to process idempotent event {id}", eventId);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in PaymentConsumer loop");
            }
        }
    }
}
