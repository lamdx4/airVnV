using Mediator;

namespace Airbnb.BookingService.Features.AdminConfirmBooking;

public record Request(Guid BookingId) : ICommand;
