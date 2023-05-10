using Simulation.Shared.Models;

namespace Simulation.Shared;

public interface IRepository
{
    IQueryable<Store>   Stores();
    IQueryable<Product> Products();
    IQueryable<Province> Provinces();
    IQueryable<Regency> Regencies();
}