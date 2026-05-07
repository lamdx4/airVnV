using Airbnb.BookingService.Infrastructure.HttpClients;
using Airbnb.SharedKernel.Events;
using MassTransit;

namespace Airbnb.BookingService.Features.MasterData;

public class MasterDataCacheInvalidationConsumer(
    PropertyServiceClient propertyServiceClient, 
    ILogger<MasterDataCacheInvalidationConsumer> logger) 
    : IConsumer<CountryToggledEvent>,
      IConsumer<TaxUpdatedEvent>,
      IConsumer<PaymentGatewayUpdatedEvent>
{
    public Task Consume(ConsumeContext<CountryToggledEvent> context)
    {
        logger.LogInformation("Invalidating cache for Country {CountryCode} due to CountryToggledEvent", context.Message.CountryCode);
        propertyServiceClient.InvalidateCountryMasterDataCache(context.Message.CountryCode);
        return Task.CompletedTask;
    }

    public Task Consume(ConsumeContext<TaxUpdatedEvent> context)
    {
        logger.LogInformation("Invalidating cache for Country {CountryCode} due to TaxUpdatedEvent", context.Message.CountryCode);
        propertyServiceClient.InvalidateCountryMasterDataCache(context.Message.CountryCode);
        return Task.CompletedTask;
    }

    public Task Consume(ConsumeContext<PaymentGatewayUpdatedEvent> context)
    {
        logger.LogInformation("Invalidating cache for Country {CountryCode} due to PaymentGatewayUpdatedEvent", context.Message.CountryCode);
        propertyServiceClient.InvalidateCountryMasterDataCache(context.Message.CountryCode);
        return Task.CompletedTask;
    }
}
