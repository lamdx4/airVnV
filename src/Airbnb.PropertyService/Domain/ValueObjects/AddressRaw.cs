using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.PropertyService.Domain.ValueObjects;

[Owned]
public record AddressRaw
{
    public string StreetAddress { get; init; } = default!;
    public string? Unit { get; init; }
    public string? PostalCode { get; init; }
    public Dictionary<string, string>? SubDivisions { get; init; }
    public AddressNotes? Notes { get; init; }

    public AddressRaw() { }

    public AddressRaw(string streetAddress, string? unit, string? postalCode, Dictionary<string, string>? SubDivisions, AddressNotes? Notes)
    {
        StreetAddress = streetAddress;
        Unit = unit;
        PostalCode = postalCode;
        this.SubDivisions = SubDivisions;
        this.Notes = Notes;
    }
}

[Owned]
public record AddressNotes
{
    public string? Public { get; init; }
    public string? Internal { get; init; }

    public AddressNotes() { }

    public AddressNotes(string? Public, string? Internal)
    {
        this.Public = Public;
        this.Internal = Internal;
    }
}
