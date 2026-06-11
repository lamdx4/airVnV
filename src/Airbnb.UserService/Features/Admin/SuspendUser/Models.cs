using FastEndpoints;
using FluentValidation;

namespace Airbnb.UserService.Features.Admin.SuspendUser;

public record Request(Guid Id, string Reason) : Mediator.ICommand<UserActionResponse>;

public record UserActionResponse(Guid Id, string Status, string Message);

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(500);
    }
}
