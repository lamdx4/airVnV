using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Account.RevokeSession;

public record Request(Guid SessionId) : Mediator.ICommand<ApiResponse<bool>>;
