using FastEndpoints;
using Elastic.Clients.Elasticsearch;
using Airbnb.SearchService.Domain;

namespace Airbnb.SearchService.Features.SearchProperties;

public record Request(string? Query, decimal? MinPrice, decimal? MaxPrice);
public record Response(List<PropertyDoc> Results);

public class Endpoint : FastEndpoints.Endpoint<Request, Response>
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
                    .Should(sh => sh.Match(m => m.Field(f => f.Name).Query(req.Query ?? "")))
                    .Filter(f => f.Range(r => r.Number(nr => nr.Field(f => f.PricePerNight).Gte((double?)(req.MinPrice ?? 0)).Lte((double?)(req.MaxPrice ?? 1000000)))))
                )
            ), ct);

        if (searchResponse.IsValidResponse)
        {
            Response = new Response(searchResponse.Documents.ToList());
        }
        else
        {
            // For error responses, gán Response property doesn't work the same as SendErrorsAsync
            // But since SendErrorsAsync is not working, I'll throw or use a different way
            throw new Exception("Search failed");
        }
    }
}
