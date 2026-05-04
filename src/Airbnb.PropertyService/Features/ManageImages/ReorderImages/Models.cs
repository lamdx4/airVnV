using Mediator;
using Airbnb.ServiceDefaults.Infrastructure;
using FastEndpoints;
using FluentValidation;

namespace Airbnb.PropertyService.Features.ManageImages.ReorderImages;

public record ImageOrderUpdate(Guid ImageId, int DisplayOrder);

public record Request(Guid PropertyId, List<ImageOrderUpdate> Orders) : Mediator.ICommand<ApiResponse<bool>>
{
    [FromHeader("X-User-Id")]
    public Guid RequesterId { get; init; }
}

public class Validator : FastEndpoints.Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.PropertyId).NotEmpty();
        RuleFor(x => x.Orders).NotEmpty().WithMessage("At least one image order must be provided.");
        RuleForEach(x => x.Orders).ChildRules(order => {
            order.RuleFor(o => o.ImageId).NotEmpty();
            order.RuleFor(o => o.DisplayOrder).GreaterThanOrEqualTo(0);
        });
    }
}
