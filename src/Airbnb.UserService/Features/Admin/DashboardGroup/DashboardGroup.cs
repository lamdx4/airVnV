using FastEndpoints;

namespace Airbnb.UserService.Features.Admin;

public class DashboardGroup : Group
{
    public DashboardGroup()
    {
        Configure("/api/admin/dashboard", ep =>
        {
            ep.Description(x => x.WithTags("Admin - Dashboard"));
        });
    }
}
