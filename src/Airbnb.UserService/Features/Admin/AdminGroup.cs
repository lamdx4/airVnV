using FastEndpoints;

namespace Airbnb.UserService.Features.Admin;

public class AdminGroup : Group
{
    public AdminGroup()
    {
        Configure("/api/admin/users", ep =>
        {
            ep.Description(x => x.WithTags("Admin - Users"));
        });
    }
}
