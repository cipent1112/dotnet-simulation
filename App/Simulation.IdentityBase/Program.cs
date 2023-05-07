using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Simulation.IdentityBase.IdentityPolicy;
using Simulation.IdentityBase.Models;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddDbContext<AppIdentityDbContext>(options => options.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]));
services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<AppIdentityDbContext>().AddDefaultTokenProviders();
services.Configure<IdentityOptions>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    options.Password.RequiredLength = 8;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
});
services.AddTransient<IPasswordValidator<AppUser>, CustomPasswordPolicy>();
services.AddTransient<IUserValidator<AppUser>, CustomUsernameEmailPolicy>();
services.AddControllers();

var app = builder.Build();
if (app.Environment.IsDevelopment()) Console.WriteLine("You are running in `Development` environment.");

app.UseRouting();
app.UseAuthorization();
app.UseAuthentication();
app.UseEndpoints(endpoints => endpoints.MapControllers());
app.Run();