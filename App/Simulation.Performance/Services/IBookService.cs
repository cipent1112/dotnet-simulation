using Simulation.Performance.Models.Entities;

namespace Simulation.Performance.Services;

public interface IBookService
{
    Task<List<Book>> GetAllAsync();
    Task<Book?>      GetOneAsync(string id);
}