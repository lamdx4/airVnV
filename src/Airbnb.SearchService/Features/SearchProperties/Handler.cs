using Mediator;
using Elastic.Clients.Elasticsearch;
using Airbnb.SearchService.Domain;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.SearchService.Features.SearchProperties;

public class Handler(ElasticsearchClient elasticClient) : IQueryHandler<Request, PagedResponse<PropertyDoc>>
{
    public async ValueTask<PagedResponse<PropertyDoc>> Handle(Request req, CancellationToken ct)
    {
        // Phân trang
        var from = (req.Page - 1) * req.PageSize;

        var searchResponse = await elasticClient.SearchAsync<PropertyDoc>(s => s
            .Indices("properties")
            .From(from)
            .Size(req.PageSize)
            .Query(q => q
                .Bool(b => b
                    .Filter(f => f
                        .GeoDistance(g => g
                            .Field("location")
                            .Distance($"{req.RadiusKm}km")
                            .Location($"{req.Latitude},{req.Longitude}")
                        )
                    )
                )
            )
            // Có thể sort theo khoảng cách:
            .Sort(so => so
                .GeoDistance(gd => gd
                    .Field("location")
                    .Location($"{req.Latitude},{req.Longitude}")
                    .Order(SortOrder.Asc)
                    .Unit(DistanceUnit.Kilometers)
                )
            ), ct);

        if (!searchResponse.IsValidResponse)
        {
            throw new BusinessException("Search failed: " + searchResponse.DebugInformation, "SEARCH_ERROR");
        }

        return new PagedResponse<PropertyDoc>(
            TotalCount: searchResponse.Total,
            Page: req.Page,
            PageSize: req.PageSize,
            Results: searchResponse.Documents.ToList()
        );
    }
}
