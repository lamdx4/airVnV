namespace Airbnb.SharedKernel.Events;

public record CountryToggledEvent(string CountryCode, bool IsSupported);
public record TaxUpdatedEvent(string CountryCode);
public record PaymentGatewayUpdatedEvent(string CountryCode);
