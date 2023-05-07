namespace Simulation.Performance.Models;

public class Response
{
    public Response(string message, object? data)
    {
        Message = message;
        Data    = data ?? new object();
    }

    public string  Message { get; set; }
    public object? Data    { get; set; }
}