using Simulation.Shared.Models;

namespace Simulation.Shared;

public interface IRepository
{
    IQueryable<Province> Provinces();
    IQueryable<Regency> Regencies();
    IQueryable<District> Districts();
}