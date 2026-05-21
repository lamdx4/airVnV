using Airbnb.PropertyService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.PropertyService.Infrastructure;

public static class DbSeeder
{
    public static async Task SeedAddressConfigsAsync(AppDbContext db)
    {
        // 1. Seed cấu hình cho Việt Nam (VN)
        var vn = await db.Countries.FirstOrDefaultAsync(c => c.Code == "VN");
        if (vn == null)
        {
            vn = Country.Create("VN", "Vietnam", "VND", 21.0285, 105.8542, isSupported: true);
            await db.Countries.AddAsync(vn);
        }
        else
        {
            vn.UpdateDefaultCoordinates(21.0285, 105.8542);
        }

        var vnConfig = new List<AddressFieldConfig>
        {
            new() { Id = "admin1", Label = "Tỉnh / Thành phố", PhotonKeys = new() { "state", "city" }, IsRequired = true },
            new() { Id = "admin2", Label = "Quận / Huyện / Phường", PhotonKeys = new() { "county", "district", "locality", "suburb", "town" }, IsRequired = true },
            new() { Id = "street", Label = "Số nhà, tên đường", PhotonKeys = new() { "street", "name" }, IsRequired = true },
            new() { Id = "unit", Label = "Căn hộ / Số phòng (Nếu có)", PhotonKeys = new(), IsRequired = false }
        };
        vn.UpdateAddressFormConfig(vnConfig);

        // 2. Seed cấu hình cho Mỹ (US)
        var us = await db.Countries.FirstOrDefaultAsync(c => c.Code == "US");
        if (us == null)
        {
            us = Country.Create("US", "United States", "USD", 37.0902, -95.7129, isSupported: true);
            await db.Countries.AddAsync(us);
        }
        else
        {
            us.UpdateDefaultCoordinates(37.0902, -95.7129);
        }

        var usConfig = new List<AddressFieldConfig>
        {
            new() { Id = "street", Label = "Street Address", PhotonKeys = new() { "street", "name" }, IsRequired = true },
            new() { Id = "admin1", Label = "State", PhotonKeys = new() { "state" }, IsRequired = true },
            new() { Id = "zipcode", Label = "Zip Code", PhotonKeys = new() { "postcode" }, IsRequired = true },
            new() { Id = "unit", Label = "Apt, Suite, Bldg", PhotonKeys = new(), IsRequired = false }
        };
        us.UpdateAddressFormConfig(usConfig);

        await db.SaveChangesAsync();
    }
}
