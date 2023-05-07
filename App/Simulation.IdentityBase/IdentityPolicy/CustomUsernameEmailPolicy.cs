using Microsoft.AspNetCore.Identity;
using Simulation.IdentityBase.Models;

namespace Simulation.IdentityBase.IdentityPolicy;

public class CustomUsernameEmailPolicy : UserValidator<AppUser>
{
    public override async Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user)
    {
        var result = await base.ValidateAsync(manager, user);
        var errors = result.Succeeded ? new List<IdentityError>() : result.Errors.ToList();

        if (user.UserName == "google")
        {
            errors.Add(new IdentityError
            {
                Description = "Google cannot be used as a user name"
            });
        }

        if (!user.Email.ToLower().EndsWith("@yahoo.com"))
        {
            errors.Add(new IdentityError
            {
                Description = "Only yahoo.com email addresses are allowed"
            });
        }

        return errors.Count == 0 ? IdentityResult.Success : IdentityResult.Failed(errors.ToArray());
    }
}