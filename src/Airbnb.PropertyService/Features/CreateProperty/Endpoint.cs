using FastEndpoints;
using Airbnb.PropertyService.Domain;
using Airbnb.PropertyService.Infrastructure;

namespace Airbnb.PropertyService.Features.CreateProperty;

public class Endpoint : FastEndpoints.Endpoint<Request, Response>
{
    private readonly AppDbContext db;
    public Endpoint(AppDbContext db) => this.db = db;

    public override void Configure()
    {
        Post("/api/properties");
        Summary(s => {
            s.Summary = "Tạo mới địa điểm lưu trú";
            s.Description = "Triển khai bằng Vertical Slice và Rich Domain Model";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var address = new AddressVO(
            req.CountryCode, 
            req.City, 
            req.StateProvince, 
            req.Ward,
            req.StreetLine1, 
            req.StreetLine2, 
            req.PostalCode, 
            req.Latitude, 
            req.Longitude
        );
        var property = new Property(req.HostId, req.Name, req.Description, req.PricePerNight, address);
        
        db.Properties.Add(property);
        await db.SaveChangesAsync(ct);
        
        Response = new Response(property.Id, property.Name);
    }
}
