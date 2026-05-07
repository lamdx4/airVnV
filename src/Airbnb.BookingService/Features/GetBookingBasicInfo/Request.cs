namespace Airbnb.BookingService.Features.GetBookingBasicInfo;

public record Request(Guid Id) : Mediator.IQuery<Response>;
