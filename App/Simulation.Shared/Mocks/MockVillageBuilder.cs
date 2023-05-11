using Simulation.Shared.Models;

namespace Simulation.Shared.Mocks;

public class MockVillageBuilder
{
    private readonly Village _village;

    public MockVillageBuilder(District district)
    {
        _village = new Village { DistrictId = district.Id };
    }

    public MockVillageBuilder WithName(string name)
    {
        _village.Name = name;
        return this;
    }

    public MockVillageBuilder WithPostalCode(string postalCode)
    {
        _village.PostalCode = postalCode;
        return this;
    }

    public MockVillageBuilder WithStatus(string status = Regency.StatusActive)
    {
        _village.Status = status;
        return this;
    }

    public Village Build() => _village;
}