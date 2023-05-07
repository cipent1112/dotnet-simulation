using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Simulation.IdentityBase.Models;

namespace Simulation.IdentityBase.Controllers;

[ApiController]
[Route("{controller}")]
public class AdminController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IPasswordHasher<AppUser> _passwordHasher;
    private readonly IPasswordValidator<AppUser> _passwordValidator;
    private readonly IUserValidator<AppUser> _userValidator;

    public AdminController(UserManager<AppUser> usrMgr, IPasswordHasher<AppUser> passwordHash, IPasswordValidator<AppUser> passwordVal, IUserValidator<AppUser> userValid)
    {
        _userManager = usrMgr;
        _passwordHasher = passwordHash;
        _passwordValidator = passwordVal;
        _userValidator = userValid;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return Ok(_userManager.Users);
    }

    [HttpPost]
    public async Task<IActionResult> Create(User user)
    {
        // if (!ModelState.IsValid) return BadRequest(ValidationProblem());
        var appUser = new AppUser
        {
            UserName = user.Name,
            Email = user.Email
        };

        var validEmail = await _userValidator.ValidateAsync(_userManager, appUser);
        if (!validEmail.Succeeded) Errors(validEmail, nameof(user.Email));
        var validPass = await _passwordValidator.ValidateAsync(_userManager, appUser, user.Password);
        if (!validPass.Succeeded) Errors(validPass, nameof(user.Password));

        var result = await _userManager.CreateAsync(appUser, user.Password);
        if (result.Succeeded)
            return Ok(new
            {
                Name = "Success",
                Message = "Create user success",
                Data = user
            });

        return BadRequest(ValidationProblem());
    }

    private void Errors(IdentityResult result, string propName)
    {
        foreach (var error in result.Errors)
            ModelState.AddModelError($"{propName}", error.Description);
    }
}