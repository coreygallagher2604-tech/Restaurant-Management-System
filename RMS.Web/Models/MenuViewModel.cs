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

    public List<MenuItem> AvailableMenuItems { get; set; } = new List<MenuItem>();

    public int MenuItemCount => MenuItems?.Count ?? 0;
    public string StatusDisplay => IsActive ? "Active" : "Inactive";
    public string CreatedOnDisplay => CreatedOn.ToLongDateString();

    public Menu ToMenu()
    {
        var menu = new Menu();
        menu.Id = Id;
        menu.Name = Name;
        menu.Type = Type;
        menu.Description = Description;
        menu.IsActive = IsActive;
        menu.MenuItems = MenuItems ?? new List<MenuItem>();
        return menu;
    }

    public static MenuViewModel FromMenu(Menu m)
    {
        var vm = new MenuViewModel();
        vm.Id = m.Id;
        vm.Name = m.Name;
        vm.Type = m.Type;
        vm.Description = m.Description;
        vm.IsActive = m.IsActive;
        vm.CreatedOn = m.CreatedOn;
        vm.MenuItems = m.MenuItems ?? new List<MenuItem>();
        return vm;
    }
}
