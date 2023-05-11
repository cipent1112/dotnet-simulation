using Simulation.Shared.Models;

namespace Simulation.Shared.Mocks;

public class MockRegionBuilder
{
    private readonly Region _region;

    public MockRegionBuilder()
    {
        _region = new Region { CreatedAt = DateTime.Now };
    }

    public MockRegionBuilder WithName(string name)
    {
        _region.Name = name;
        return this;
    }

    public MockRegionBuilder WithStatus(string status = Region.StatusActive)
    {
        _region.Status = status;
        return this;
    }

    public Region Build() => _region;
}