using System.ComponentModel.DataAnnotations;
using RMS.Data.Entities;

namespace RMS.Web.Models;
    
public class UserRegisterViewModel
{       
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }

    [Compare("Password", ErrorMessage = "Confirm password doesn't match, Type again !")]
    [Display(Name = "Confirm Password")]  
    public string PasswordConfirm  { get; set; }

    [Required]
    public Role Role { get; set; }

    [Required]
    public string Name { get; set; }

}
