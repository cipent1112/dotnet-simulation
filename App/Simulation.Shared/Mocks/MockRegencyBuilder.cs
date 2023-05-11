using Simulation.Shared.Models;

namespace Simulation.Shared.Mocks;

public class MockRegencyBuider
{
    private readonly Regency _regency;

    public MockRegencyBuider(Province province)
    {
        _regency = new Regency { ProvinceId = province.Id };
    }

    public MockRegencyBuider WithName(string name)
    {
        _regency.Name = name;
        return this;
    }

    public MockRegencyBuider WitCode(string code)
    {
        _regency.Code = code;
        return this;
    }

    public MockRegencyBuider WithStatus(string status = Regency.StatusActive)
    {
        _regency.Status = status;
        return this;
    }

    public Regency Build() => _regency;
}