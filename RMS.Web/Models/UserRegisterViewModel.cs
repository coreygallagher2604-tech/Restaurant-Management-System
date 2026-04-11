using System.ComponentModel.DataAnnotations;
using RMS.Data.Entities;

namespace RMS.Web.Models;
    
public class UserRegisterViewModel
{       
    [Required]
    [RegularExpression(
        @"^[^@\s]+@[^@\s]+\.[^@\s]{2,}$",
        ErrorMessage = "Enter a valid email address (e.g. name@example.com).")]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }

    [Compare("Password", ErrorMessage = "Confirm password doesn't match, Type again !")]
    [Display(Name = "Confirm Password")]  
    public string PasswordConfirm  { get; set; }

    // Role is not exposed to the user — all self-registrations are guests
    public Role Role { get; set; } = Role.guest;

    [Required]
    public string Name { get; set; }

}
