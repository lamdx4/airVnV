using Microsoft.Extensions.Hosting;

namespace Airbnb.AppHost.Models;

public record AppMicroservices(
    IResourceBuilder<ProjectResource> PropSvc,
    IResourceBuilder<ProjectResource> BookSvc,
    IResourceBuilder<ProjectResource> UserSvc,
    IResourceBuilder<ProjectResource> PaySvc,
    IResourceBuilder<ProjectResource> SearchSvc,
    IResourceBuilder<ProjectResource> ChatSvc
);
