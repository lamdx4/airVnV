using Airbnb.AppHost.Models;
using Microsoft.Extensions.Hosting;

namespace Airbnb.AppHost.Extensions;

public static class MicroserviceExtensions
{
    public static AppMicroservices AddMicroservices(this IDistributedApplicationBuilder builder, AppInfrastructure infra)
    {
        var propSvc = builder.AddProject<Projects.Airbnb_PropertyService>("propertyservice")
            .WithDefaultServiceConfig()
            .WithReference(infra.PropDb)
            .WithReference(infra.RabbitMq)
            .WaitFor(infra.PropDb)
            .WaitFor(infra.RabbitMq);

        var bookSvc = builder.AddProject<Projects.Airbnb_BookingService>("bookingservice")
            .WithDefaultServiceConfig()
            .WithReference(infra.BookDb)
            .WithReference(infra.RabbitMq)
            .WithReference(infra.Kafka)
            .WithReference(propSvc)
            .WaitFor(infra.BookDb)
            .WaitFor(infra.RabbitMq)
            .WaitFor(infra.Kafka);

        var userSvc = builder.AddProject<Projects.Airbnb_UserService>("userservice")
            .WithDefaultServiceConfig()
            .WithReference(infra.UserDb)
            .WithReference(infra.RabbitMq)
            .WithReference(propSvc)   // needed for dashboard: property stats + recent activity
            .WithReference(bookSvc)   // needed for dashboard: booking stats + revenue chart
            .WaitFor(infra.UserDb)
            .WaitFor(infra.RabbitMq);

        var paySvc = builder.AddProject<Projects.Airbnb_PaymentService>("paymentservice")
            .WithDefaultServiceConfig()
            .WithReference(infra.PayDb)
            .WithReference(infra.RabbitMq)
            .WithReference(propSvc)   // for country master-data (tax, gateway)
            .WithReference(userSvc)   // for host basic info lookup
            .WithReference(bookSvc)   // for booking → guest lookup on admin payments
            .WaitFor(infra.PayDb)
            .WaitFor(infra.RabbitMq);

        var searchSvc = builder.AddProject<Projects.Airbnb_SearchService>("searchservice")
            .WithDefaultServiceConfig()
            .WithReference(infra.Elasticsearch)
            .WithReference(infra.Kafka)
            .WithReference(infra.Redis)
            .WaitFor(infra.Elasticsearch)
            .WaitFor(infra.Kafka)
            .WaitFor(infra.Redis);

        var chatSvc = builder.AddProject<Projects.Airbnb_ChatService>("chatservice")
            .WithDefaultServiceConfig()
            .WithReference(infra.ChatDb)
            .WithReference(infra.RabbitMq)
            .WithReference(infra.Redis)
            .WithReference(propSvc)
            .WithReference(userSvc)
            .WaitFor(infra.ChatDb)
            .WaitFor(infra.RabbitMq)
            .WaitFor(infra.Redis);

        return new AppMicroservices(
            propSvc,
            bookSvc,
            userSvc,
            paySvc,
            searchSvc,
            chatSvc
        );
}
}
