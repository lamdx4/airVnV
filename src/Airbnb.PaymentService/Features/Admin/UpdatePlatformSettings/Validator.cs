using FastEndpoints;
using FluentValidation;

namespace Airbnb.PaymentService.Features.Admin.UpdatePlatformSettings;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.PlatformFeePercent)
            .InclusiveBetween(0m, 1m)
            .WithMessage("PlatformFeePercent must be between 0 and 1 (e.g. 0.10 = 10%).");
        RuleFor(x => x.MinPayoutAmount)
            .GreaterThanOrEqualTo(0m)
            .WithMessage("MinPayoutAmount must be ≥ 0.");
        RuleFor(x => x.DefaultCurrency)
            .NotEmpty().WithMessage("DefaultCurrency is required.")
            .Length(3).WithMessage("DefaultCurrency must be a 3-letter ISO code.");
    }
}
