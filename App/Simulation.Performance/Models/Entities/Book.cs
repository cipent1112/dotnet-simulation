using System.ComponentModel.DataAnnotations;

namespace Simulation.Performance.Models.Entities;

public class Book
{
    public Book()
    {
        Id = Guid.NewGuid().ToString().ToLower();
    }

    [Key] [StringLength(36)] public string  Id          { get; set; }
    public                          string  Author      { get; set; } = null!;
    public                          string  Name        { get; set; } = null!;
    public                          string? Description { get; set; }
}