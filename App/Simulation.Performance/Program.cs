using Microsoft.EntityFrameworkCore;
using Simulation.Performance;
using Simulation.Performance.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(_ => _.UseSqlServer(connectionString));
// builder.Services.AddDbContext<AppDbContext>(_ => _.UseInMemoryDatabase("Simulation.Performance"));

builder.Services.AddControllers();
builder.Services.AddRouting(_ => _.LowercaseUrls = true);
builder.Services.AddScoped<IBookService, BookService>();

var app = builder.Build();
app.UseRouting();
app.UseEndpoints(e => e.MapControllers());

var scope = app.Services.CreateScope();
var db    = scope.ServiceProvider.GetRequiredService<AppDbContext>();
if ((await db.Database.GetPendingMigrationsAsync()).Any()) await db.Database.MigrateAsync();

app.Run();