using System.ComponentModel.DataAnnotations;
using RMS.Data.Entities;

namespace RMS.Web.Models;

public class UserViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = "";

    public string Email { get; set; } = "";

    public Role Role { get; set; }

    public static UserViewModel FromUser(User u)
    {
        return new UserViewModel
        {
            Id = u.Id,
            Name = u.Name,
            Email = u.Email,
            Role = u.Role
        };
    }
}
