using Simulation.Shared.Models;

namespace Simulation.Shared.Mocks;

public class MockProvinceAssignmentBuilder
{
    private readonly ProvinceAssignment _provinceAssignment;

    public MockProvinceAssignmentBuilder(Province province)
    {
        _provinceAssignment = new ProvinceAssignment { ProvinceId = province.Id };
    }

    public MockProvinceAssignmentBuilder WithAssignmentStatus(string ass = ProvinceAssignment.AssignmentStatusNew)
    {
        _provinceAssignment.AssignmentStatus = ass;
        return this;
    }

    public MockProvinceAssignmentBuilder WithStatus(string status = ProvinceAssignment.StatusActive)
    {
        _provinceAssignment.Status = status;
        return this;
    }

    public ProvinceAssignment Build() => _provinceAssignment;
}