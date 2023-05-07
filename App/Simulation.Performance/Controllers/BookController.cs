using Microsoft.AspNetCore.Mvc;
using Simulation.Performance.Models;
using Simulation.Performance.Models.Entities;
using Simulation.Performance.Services;

namespace Simulation.Performance.Controllers;

[ApiController, Route("book")]
public class BookController : ControllerBase
{
    private readonly IBookService _bookService;

    public BookController(IBookService bookService)
    {
        _bookService = bookService;
    }

    [HttpGet("completed")]
    public async Task<IActionResult> Completed()
    {
        var books = await _bookService.GetAllAsync();
        return Ok(new Response(
            message: $"{books.Count} books loaded successfully.",
            data: books.Select(b => new
            {
                b.Id,
                b.Author,
                b.Name,
                ShortDescription = b.Description?.Length > 50 ? b.Description?[..50] + " ..." : b.Description
            }).AsEnumerable()
        ));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> One(string id)
    {
        var book = await _bookService.GetOneAsync(id);
        if (book is null) return NotFound($"Book with id `{id}` does`nt exist.");
        return Ok(new
        {
            message = $"Book `{book.Name}` loaded successfully.",
            data    = book
        });
    }
}