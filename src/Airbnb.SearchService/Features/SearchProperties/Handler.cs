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

        var hasLocation = req.Latitude.HasValue && req.Longitude.HasValue;
        LatLonGeoLocation? geoPoint = hasLocation 
            ? new LatLonGeoLocation { Lat = req.Latitude!.Value, Lon = req.Longitude!.Value }
            : null;

        var searchResponse = await elasticClient.SearchAsync<PropertyDoc>(s => {
            s.Indices("properties")
             .TrackTotalHits(new TrackHits(true))
             .From(from)
             .Size(pageSize)
             .Query(q => q
                .Bool(b => {
                    if (hasLocation)
                    {
                        b.Filter(f => f
                            .GeoDistance(g => g
                                .Field(p => p.Location)
                                .Distance($"{req.RadiusKm}km")
                                .Location(geoPoint!)
                            )
                        );
                    }
                    if (req.PropertyType.HasValue)
                    {
                        b.Must(m => m
                            .Term(t => t
                                .Field(p => p.PropertyType)
                                .Value(req.PropertyType.Value)
                            )
                        );
                    }
                })
             );

            if (hasLocation)
            {
                s.Sort(so => so
                    .GeoDistance(gd => gd
                        .Field(p => p.Location)
                        .Location(geoPoint!)
                        .Order(SortOrder.Asc)
                        .Unit(DistanceUnit.Kilometers)
                    )
                );
            }
            else
            {
                s.Sort(so => so
                    .Field(f => f.CreatedAt, fsd => fsd.Order(SortOrder.Desc))
                );
            }
        }, ct);

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
