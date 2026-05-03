namespace Airbnb.PropertyService.Domain.Entities;

/// <summary>
/// Bảng chuẩn hóa địa phận hành chính – Entity, không phải Value Object.
/// Thêm country mới chỉ cần insert data, không sửa schema.
/// </summary>
public class AdminDivision
{
    public string Code { get; private set; } = default!;     // PK – "VN-HCM"
    public string? ParentCode { get; private set; }           // FK to self
    public string CountryCode { get; private set; } = default!;
    public short Level { get; private set; }                  // 1=tỉnh, 2=huyện
    public string NameLocal { get; private set; } = default!;
    public string? NameEn { get; private set; }
    public string[] Aliases { get; private set; } = Array.Empty<string>(); // TEXT[] – "Saigon" -> HCM
}
