using FastEndpoints;
using FluentValidation;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Admin.BanUser;

public record Request(Guid Id, string Reason) : ICommand<ApiResponse<BanUserResponse>>;

public record BanUserResponse(Guid Id, string Status, string Message);

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(500);
    }
}
