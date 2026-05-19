using FastEndpoints;
using FluentValidation;
using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.UserService.Features.Account.ChangePassword;

public record Request(
    string CurrentPassword,
    string NewPassword,
    string ConfirmPassword
) : Mediator.ICommand<ApiResponse<bool>>;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.CurrentPassword).NotEmpty();
        RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(6);
        RuleFor(x => x.ConfirmPassword).Equal(x => x.NewPassword).WithMessage("Mật khẩu xác nhận không khớp");
    }
}
