using Microsoft.EntityFrameworkCore;
using Simulation.Performance.Models.Entities;

namespace Simulation.Performance.Services;

public class BookService : IBookService
{
    private readonly AppDbContext _db;

    public BookService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Book>> GetAllAsync()
    {
        return await _db.Book.ToListAsync();
    }

    public async Task<Book?> GetOneAsync(string id)
    {
        return await _db.Book.FindAsync(id);
    }
}