using Airbnb.PaymentService.Domain;

namespace Airbnb.PaymentService.Tests;

/// <summary>
/// Tests for PlatformFeeConfig domain — mapped to UC-C3 (Platform Fee Configuration) acceptance criteria
/// </summary>
public class PlatformFeeConfigDomainTests
{
    [Fact]
    public void Create_Valid_Fee_Succeeds()
    {
        // UC-C3 AC-3: Save updates the fee with validation
        var config = PlatformFeeConfig.Create(10.0m, Guid.NewGuid());

        Assert.Equal(10.0m, config.FeePercentage);
        Assert.True(config.IsActive);
    }

    [Fact]
    public void Create_Fee_Above_50_Throws()
    {
        // UC-C3 AC-4: Validation rejects values above 50%
        Assert.Throws<ArgumentException>(() =>
            PlatformFeeConfig.Create(55m, Guid.NewGuid()));
    }

    [Fact]
    public void Create_Fee_Below_0_Throws()
    {
        // UC-C3 AC-4: Validation rejects negative values
        Assert.Throws<ArgumentException>(() =>
            PlatformFeeConfig.Create(-5m, Guid.NewGuid()));
    }

    [Fact]
    public void Create_Fee_At_Boundary_0_Succeeds()
    {
        // UC-C3 AC-4: 0% is valid
        var config = PlatformFeeConfig.Create(0m, Guid.NewGuid());
        Assert.Equal(0m, config.FeePercentage);
    }

    [Fact]
    public void Create_Fee_At_Boundary_50_Succeeds()
    {
        // UC-C3 AC-4: 50% is valid
        var config = PlatformFeeConfig.Create(50m, Guid.NewGuid());
        Assert.Equal(50m, config.FeePercentage);
    }

    [Fact]
    public void Create_With_PreviousValue_Records_Change()
    {
        // UC-C3 AC-8: Audit trail is created for fee changes
        var previousValue = 10.0m;
        var config = PlatformFeeConfig.Create(12.5m, Guid.NewGuid(), previousValue, "Increase for Q3");

        Assert.Equal(12.5m, config.FeePercentage);
        Assert.Equal(previousValue, config.PreviousValue);
        Assert.Equal("Increase for Q3", config.Description);
    }

    [Fact]
    public void Deactivate_Sets_IsActive_False()
    {
        // BR-003-R16: Previous config deactivated on new creation
        var config = PlatformFeeConfig.Create(10.0m, Guid.NewGuid());
        config.Deactivate();

        Assert.False(config.IsActive);
    }

    [Fact]
    public void Create_With_Empty_ChangedBy_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            PlatformFeeConfig.Create(10.0m, Guid.Empty));
    }
}
