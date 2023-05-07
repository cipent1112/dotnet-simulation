using System.ComponentModel.DataAnnotations;

namespace Simulation.IdentityBase.Models;

public class User
{
    [Required] 
    public string Name { get; set; } = null!;

    [Required]
    [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
    public string Email { get; set; } = null!;
 
    [Required]
    public string Password { get; set; } = null!;
}