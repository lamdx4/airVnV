using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Airbnb.Infrastructure.Media;

public static class DependencyInjection
{
    public static IServiceCollection AddMediaServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MediaOptions>(configuration.GetSection(MediaOptions.SectionName));
        var useMock = configuration.GetValue<bool>("Media:UseMock");
        if (useMock)
        {
            services.AddSingleton<IMediaProvider, MockMediaProvider>();
        }
        else
        {
            services.AddSingleton<IMediaProvider, CloudinaryMediaProvider>();
        }

        return services;
    }
}
