using Simulation.Shared.Models;

namespace Simulation.Shared.Mocks;

public class MockProvinceBuilder
{
    private readonly Province _province;

    public MockProvinceBuilder()
    {
        _province = new Province();
    }

    public MockProvinceBuilder WithName(string name)
    {
        _province.Name = name;
        return this;
    }

    public MockProvinceBuilder WithStatus(string status = Province.StatusActive)
    {
        _province.Status = status;
        return this;
    }

    public Province Build() => _province;
}