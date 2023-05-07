using Bogus;
using Microsoft.EntityFrameworkCore;
using Simulation.Performance.Models.Entities;

namespace Simulation.Performance;

public sealed class AppDbContext : DbContext
{
    public DbSet<Book> Book { get; set; } = null!;

    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var books = new Faker<Book>()
            .RuleFor(_ => _.Name, _ => _.Name.JobTitle())
            .RuleFor(_ => _.Description, _ => _.Lorem.Paragraph(min: 100))
            .RuleFor(_ => _.Author, _ => _.Person.FullName);

        modelBuilder.Entity<Book>().HasData(books.GenerateBetween(500, 500));
    }
}