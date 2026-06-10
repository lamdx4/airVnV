using FastEndpoints;
using FluentValidation;

namespace Airbnb.PaymentService.Features.Admin.Reports.GetRevenueSeries;

public class Validator : Validator<Request>
{
    private static readonly HashSet<string> AllowedGroups = new(StringComparer.OrdinalIgnoreCase)
    { "day", "week", "month" };

    public Validator()
    {
        RuleFor(x => x.From)
            .NotEmpty().WithMessage("From date is required.")
            .Must(s => DateOnly.TryParse(s, out _)).WithMessage("From must be yyyy-MM-dd.");
        RuleFor(x => x.To)
            .NotEmpty().WithMessage("To date is required.")
            .Must(s => DateOnly.TryParse(s, out _)).WithMessage("To must be yyyy-MM-dd.");
        RuleFor(x => x.GroupBy)
            .Must(g => AllowedGroups.Contains(g))
            .WithMessage("GroupBy must be one of: day, week, month.");
        RuleFor(x => x.Currency)
            .Must(c => c is null || c.Length == 3)
            .WithMessage("Currency must be a 3-letter ISO code (e.g. USD, VND).");
        RuleFor(x => x)
            .Must(x => !DateOnly.TryParse(x.From, out var f) || !DateOnly.TryParse(x.To, out var t) || f <= t)
            .WithMessage("From must be on or before To.");
    }
}
