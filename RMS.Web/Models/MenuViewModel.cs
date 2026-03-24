using System.ComponentModel.DataAnnotations;
using RMS.Data.Entities;

namespace RMS.Web.Models;

public class MenuViewModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; set; }

    [Required]
    [StringLength(50)]
    public string Type { get; set; }

    [DataType(DataType.Date)]
    public DateTime CreatedOn { get; set; }

    [Required]
    [StringLength(500)]
    public string Description { get; set; }

    public bool IsActive { get; set; } = true;

    public List<MenuItem> MenuItems { get; set; } = new List<MenuItem>();

    public int MenuItemCount => MenuItems?.Count ?? 0;
    public string StatusDisplay => IsActive ? "Active" : "Inactive";
    public string CreatedOnDisplay => CreatedOn.ToLongDateString();

    public Menu ToMenu() => new Menu
    {
        Id = Id,
        Name = Name,
        Type = Type,
        Description = Description,
        IsActive = IsActive,
        MenuItems = MenuItems ?? new List<MenuItem>()
    };

    public static MenuViewModel FromMenu(Menu m) => new MenuViewModel
    {
        Id = m.Id,
        Name = m.Name,
        Type = m.Type,
        Description = m.Description,
        IsActive = m.IsActive,
        CreatedOn = m.CreatedOn,
        MenuItems = m.MenuItems ?? new List<MenuItem>()
    };
}
