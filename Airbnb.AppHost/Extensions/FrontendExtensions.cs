using Microsoft.Extensions.Hosting;

namespace Airbnb.AppHost.Extensions;

public static class FrontendExtensions
{
    public static IDistributedApplicationBuilder AddFrontends(
        this IDistributedApplicationBuilder builder,
        IResourceBuilder<ProjectResource> gateway)
    {
        // 1. React Web App (Vite)
        builder.AddDockerfile("airbnb-web", "../airbnb-web")
            .WithHttpEndpoint(port: 3001, targetPort: 80, name: "http")
            .WithReference(gateway)
            .WithEnvironment("VITE_API_GATEWAY_URL", gateway.GetEndpoint("http"));

        // 2. Next.js Admin Panel
        builder.AddDockerfile("airbnb-admin", "../airbnb-admin")
            .WithHttpEndpoint(port: 9999, targetPort: 3000, name: "http")
            .WithReference(gateway)
            .WithEnvironment("NEXT_PUBLIC_API_GATEWAY_URL", gateway.GetEndpoint("http"));

        return builder;
    }
}
