using FastEndpoints;
using FluentValidation;

namespace Airbnb.PaymentService.Features.Admin.Reports.GetRevenueOverview;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.From)
            .NotEmpty().WithMessage("From date is required.")
            .Must(s => DateOnly.TryParse(s, out _)).WithMessage("From must be yyyy-MM-dd.");
        RuleFor(x => x.To)
            .NotEmpty().WithMessage("To date is required.")
            .Must(s => DateOnly.TryParse(s, out _)).WithMessage("To must be yyyy-MM-dd.");
        RuleFor(x => x)
            .Must(x => !DateOnly.TryParse(x.From, out var f) || !DateOnly.TryParse(x.To, out var t) || f <= t)
            .WithMessage("From must be on or before To.");
    }
}
