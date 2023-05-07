using Microsoft.AspNetCore.Identity;
using Simulation.IdentityBase.Models;

namespace Simulation.IdentityBase.IdentityPolicy;

public class CustomPasswordPolicy : PasswordValidator<AppUser>
{
    public override async Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user, string password)
    {
        var result = await base.ValidateAsync(manager, user, password);
        var errors = result.Succeeded ? new List<IdentityError>() : result.Errors.ToList();

        if (password.ToLower().Contains(user.UserName.ToLower()))
        {
            errors.Add(new IdentityError
            {
                Description = "Password cannot contain username"
            });
        }

        if (password.Contains("12345678"))
        {
            errors.Add(new IdentityError
            {
                Description = "Password cannot contain numeric sequence"
            });
        }

        return errors.Count == 0 ? IdentityResult.Success : IdentityResult.Failed(errors.ToArray());
    }
}