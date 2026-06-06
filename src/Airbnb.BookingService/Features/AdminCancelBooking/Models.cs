using Mediator;

namespace Airbnb.BookingService.Features.AdminCancelBooking;

public record Request(Guid BookingId, string Reason) : ICommand;
