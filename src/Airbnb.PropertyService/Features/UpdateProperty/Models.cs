using FastEndpoints;
using FluentValidation;
using Mediator;
using Airbnb.PropertyService.Domain.ValueObjects;

namespace Airbnb.PropertyService.Features.UpdateProperty;

public record Request(
    Guid PropertyId,
    // Core info – tất cả optional (partial update)
    string? Title,
    string? Description,
    // Pricing
    decimal? BasePrice,
    string? CurrencyCode,
    decimal? CleaningFee,
    decimal? ServiceFee,
    decimal? WeekendPremiumPercent,
    // Capacity
    int? GuestCount,
    int? BedroomCount,
    int? BedCount,
    int? BathroomCount,
    // HouseRules
    bool? AllowPets,
    bool? AllowSmoking,
    bool? AllowEvents,
    TimeOnly? CheckInTime,
    TimeOnly? CheckOutTime,
    bool? FlexibleCheckOut) : Mediator.ICommand<Response>
{
    [FromHeader("X-User-Id")]
    public Guid RequesterId { get; init; }
}

public record Response(Guid Id, DateTimeOffset UpdatedAt);

public class Validator : FastEndpoints.Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.PropertyId).NotEmpty();
        RuleFor(x => x.Title).MaximumLength(255).When(x => x.Title is not null);
        RuleFor(x => x.BasePrice).GreaterThan(0).When(x => x.BasePrice.HasValue);
        RuleFor(x => x.CurrencyCode).Length(3).When(x => x.CurrencyCode is not null);
        RuleFor(x => x.GuestCount).GreaterThan(0).When(x => x.GuestCount.HasValue);
        RuleFor(x => x.WeekendPremiumPercent).GreaterThanOrEqualTo(0).When(x => x.WeekendPremiumPercent.HasValue);
    }
}
