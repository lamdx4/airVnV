using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Airbnb.BookingService.Infrastructure.Saga;
using Airbnb.SharedKernel.Events;
using Xunit;

namespace Airbnb.BookingService.Tests;

public class BookingSagaTests : IAsyncDisposable
{
    private readonly ServiceProvider _provider;
    private readonly ITestHarness _harness;

    public BookingSagaTests()
    {
        _provider = new ServiceCollection()
            .AddMassTransitTestHarness(x =>
            {
                x.AddSagaStateMachine<BookingStateMachine, BookingState>();
            })
            .BuildServiceProvider(true);

        _harness = _provider.GetRequiredService<ITestHarness>();
    }

    [Fact]
    public async Task Should_Transition_To_AwaitingPayment_When_BookingCreated()
    {
        await _harness.Start();

        var bookingId = Guid.NewGuid();
        await _harness.Bus.Publish(new BookingCreatedEvent(
            bookingId, Guid.NewGuid(), Guid.NewGuid(), 100, "USD", "VN"));

        // Verify Saga started
        Assert.True(await _harness.StateMachineSagas.Any<BookingState>(x => x.BookingId == bookingId));
        
        var instance = _harness.StateMachineSagas.GetSaga<BookingState>(bookingId);
        Assert.NotNull(instance);
        // Using string comparison for state as it's persisted as string
        Assert.Equal("AwaitingPayment", instance.CurrentState);

        // Verify InitiatePaymentCommand was sent
        Assert.True(await _harness.Sent.Any<InitiatePaymentCommand>(x => x.BookingId == bookingId));
    }

    [Fact]
    public async Task Should_Cancel_Booking_When_Timeout_Occurs()
    {
        await _harness.Start();

        var bookingId = Guid.NewGuid();
        await _harness.Bus.Publish(new BookingCreatedEvent(
            bookingId, Guid.NewGuid(), Guid.NewGuid(), 100, "USD", "VN"));

        // Advance time by 16 minutes
        await _harness.InactivityTask; // Ensure processed
        // In real MassTransit testing, we use the virtual time provider
        // but here we can just fire the timeout event manually if needed, 
        // or let the harness handle the scheduler if configured.
        
        // For simplicity in this demo test, we simulate the timeout event
        await _harness.Bus.Publish(new PaymentTimeoutEvent(bookingId));

        Assert.True(await _harness.Published.Any<BookingCancelledEvent>(x => x.BookingId == bookingId));
        
        var instance = _harness.StateMachineSagas.GetSaga<BookingState>(bookingId);
        Assert.Equal("Cancelled", instance.CurrentState);
    }

    public async ValueTask DisposeAsync()
    {
        await _provider.DisposeAsync();
    }
}
