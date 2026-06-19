using Airbnb.AppHost.Models;
using Microsoft.Extensions.Hosting;

namespace Airbnb.AppHost.Extensions;

public static class GatewayExtensions
{
    public static IResourceBuilder<ProjectResource> AddGateway(this IDistributedApplicationBuilder builder, AppMicroservices services)
    {
        var gateway = builder.AddProject<Projects.Airbnb_Gateway>("gateway")
            .WithDefaultServiceConfig()
            .WithEndpoint("http", e =>
            {
                e.Port = 8088; // Fix cứng port 8088 để tránh đụng với shopnexus trên VPS
                e.TargetPort = 8080;
                e.IsExternal = true;
            })
            .WithReference(services.UserSvc)
            .WithReference(services.PropSvc)
            .WithReference(services.BookSvc)
            .WithReference(services.PaySvc)
            .WithReference(services.SearchSvc)
            .WithReference(services.ChatSvc);

        return gateway;
    }
}
