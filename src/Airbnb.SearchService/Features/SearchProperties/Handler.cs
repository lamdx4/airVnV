using Mediator;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Core.Search;
using Airbnb.SearchService.Domain;
using Airbnb.ServiceDefaults.Infrastructure;

namespace Airbnb.SearchService.Features.SearchProperties;

public class Handler(ElasticsearchClient elasticClient) : IQueryHandler<Request, PagedResponse<PropertyDoc>>
{
    public async ValueTask<PagedResponse<PropertyDoc>> Handle(Request req, CancellationToken ct)
    {
        var page = Math.Max(req.Page, 1);
        var pageSize = Math.Clamp(req.PageSize, 1, 100);
        var from = (page - 1) * pageSize;

        var geoPoint = new LatLonGeoLocation
        {
            Lat = req.Latitude,
            Lon = req.Longitude
        };

        var searchResponse = await elasticClient.SearchAsync<PropertyDoc>(s => s
            .Indices("properties")
            .TrackTotalHits(new TrackHits(true))
            .From(from)
            .Size(pageSize)
            .Query(q => q
                .Bool(b => b
                    .Filter(f => f
                        .GeoDistance(g => g
                            .Field(p => p.Location)
                            .Distance($"{req.RadiusKm}km")
                            .Location(geoPoint)
                        )
                    )
                )
            )
            .Sort(so => so
                .GeoDistance(gd => gd
                    .Field(p => p.Location)
                    .Location(geoPoint)
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
            Page: page,
            PageSize: pageSize,
            Results: searchResponse.Documents.ToList()
        );
    }
}
