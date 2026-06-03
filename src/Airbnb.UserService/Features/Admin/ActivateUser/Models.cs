using FastEndpoints;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Admin.ActivateUser;

public record ActivateUserRequest(Guid Id) : ICommand<ApiResponse<ActivateUserResponse>>;

public record ActivateUserResponse(Guid Id, string Status, string Message);
