using Simulation.Shared.Models;

namespace Simulation.Shared.Mocks;

public class MockRegionProvinceBuilder
{
    private readonly RegionProvince _regionProvince;

    public MockRegionProvinceBuilder(Region region, Province province)
    {
        _regionProvince = new RegionProvince
        {
            RegionId = region.Id, ProvinceId = province.Id, CreatedAt = DateTime.Now
        };
    }

    public MockRegionProvinceBuilder WithStatus(string status = RegionProvince.StatusActive)
    {
        _regionProvince.Status = status;
        return this;
    }

    public RegionProvince Build() => _regionProvince;
}