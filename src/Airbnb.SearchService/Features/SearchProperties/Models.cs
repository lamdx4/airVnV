using FastEndpoints;
using Elastic.Clients.Elasticsearch;
using Airbnb.SearchService.Domain;

namespace Airbnb.SearchService.Features.SearchProperties;

public record Request(string? Query, decimal? MinPrice, decimal? MaxPrice);
public record Response(List<PropertyDoc> Results);

public class Endpoint : FastEndpoints.Endpoint<Request, Airbnb.ServiceDefaults.Infrastructure.ApiResponse<Response>>
{
    private readonly ElasticsearchClient elasticClient;
    public Endpoint(ElasticsearchClient elasticClient) => this.elasticClient = elasticClient;

    public override void Configure()
    {
        Get("/api/search");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var searchResponse = await elasticClient.SearchAsync<PropertyDoc>(s => s
            .Indices("properties")
            .Query(q => q
                .Bool(b => b
                    .Should(sh => sh.Match(m => m.Field(f => f.Title).Query(req.Query ?? "")))
                    .Filter(f => f.Range(r => r.Number(nr => nr.Field(f => f.BasePrice).Gte((double?)(req.MinPrice ?? 0)).Lte((double?)(req.MaxPrice ?? 1000000)))))
                )
            ), ct);

        if (searchResponse.IsValidResponse)
        {
            var result = new Response(searchResponse.Documents.ToList());
            await Send.ResponseAsync(Airbnb.ServiceDefaults.Infrastructure.ApiResponse<Response>.SuccessResult(result), cancellation: ct);
        }
        else
        {
            await Send.ResponseAsync(Airbnb.ServiceDefaults.Infrastructure.ApiResponse<Response>.FailureResult("SEARCH_ERROR", "Search failed: " + searchResponse.DebugInformation), 500, ct);
        }
    }
}
