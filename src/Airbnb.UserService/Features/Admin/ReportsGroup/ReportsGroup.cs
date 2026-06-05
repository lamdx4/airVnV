using FastEndpoints;

namespace Airbnb.UserService.Features.Admin;

public class ReportsGroup : Group
{
    public ReportsGroup()
    {
        Configure("/api/admin/reports", ep =>
        {
            ep.Description(x => x.WithTags("Admin - Reports"));
        });
    }
}
