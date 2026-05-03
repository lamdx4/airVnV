using FastEndpoints;

namespace Airbnb.UserService.Features;

public class AuthGroup : Group
{
    public AuthGroup()
    {
        Configure("/api/users", ep =>
        {
            ep.Description(x => x.WithTags("Users"));
        });
    }
}
