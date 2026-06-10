namespace Airbnb.UserService.Features.Admin.ActivateUser;

public record ActivateUserRequest(Guid Id) : Mediator.ICommand<ActivateUserResponse>;

public record ActivateUserResponse(Guid Id, string Status, string Message);
