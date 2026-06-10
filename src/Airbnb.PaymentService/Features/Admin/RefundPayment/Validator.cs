using FastEndpoints;
using FluentValidation;

namespace Airbnb.PaymentService.Features.Admin.RefundPayment;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Payment Id is required.");
        RuleFor(x => x.Amount).GreaterThan(0m).WithMessage("Amount must be greater than 0.");
        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Reason is required.")
            .MaximumLength(500);
    }
}
