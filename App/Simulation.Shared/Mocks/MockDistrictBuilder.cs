using Simulation.Shared.Models;

namespace Simulation.Shared.Mocks;

public class MockDistrictBuilder
{
    private readonly District _district;

    public MockDistrictBuilder(Regency regency)
    {
        _district = new District { RegencyId = regency.Id };
    }

    public MockDistrictBuilder WithName(string name)
    {
        _district.Name = name;
        return this;
    }

    public MockDistrictBuilder WithStatus(string status = District.StatusActive)
    {
        _district.Status = status;
        return this;
    }

    public District Build() => _district;
}