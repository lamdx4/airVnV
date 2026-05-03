namespace Airbnb.PropertyService.Domain.ValueObjects;

public record AddressRaw(
    string StreetAddress,
    string? Unit,
    string? PostalCode,
    Dictionary<string, string>? SubDivisions, // level3, level4 linh hoạt
    AddressNotes? Notes
);

public record AddressNotes(
    string? Public,   // Hiển thị cho khách
    string? Internal  // Nội bộ host – không leak ra FE
);
