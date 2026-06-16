using Airbnb.AppHost.Models;
using Microsoft.Extensions.Hosting;

namespace Airbnb.AppHost.Extensions;

public static class GatewayExtensions
{
    public static IResourceBuilder<ProjectResource> AddGateway(this IDistributedApplicationBuilder builder, AppMicroservices services)
    {
        var gateway = builder.AddProject<Projects.Airbnb_Gateway>("gateway")
            .WithDefaultServiceConfig()
            .WithReference(services.UserSvc)
            .WithReference(services.PropSvc)
            .WithReference(services.BookSvc)
            .WithReference(services.PaySvc)
            .WithReference(services.SearchSvc)
            .WithReference(services.ChatSvc);

        return gateway;
    }
}
