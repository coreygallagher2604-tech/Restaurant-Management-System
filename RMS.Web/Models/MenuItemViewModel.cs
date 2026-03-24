using System.ComponentModel.DataAnnotations;
using RMS.Data.Entities;

namespace RMS.Web.Models;

public class MenuItemViewModel
{
    public int Id { get; set; }

    public int? MenuID { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; set; }

    [StringLength(500)]
    public string Description { get; set; }

    [Required]
    [Range(0.01, 10000)]
    [DisplayFormat(DataFormatString = "{0:F2}")]
    public double Price { get; set; }

    public MenuItem ToMenuItem() => new MenuItem
    {
        Id = this.Id,
        MenuID = this.MenuID,
        Name = this.Name,
        Description = this.Description,
        Price = this.Price,
    };

    public static MenuItemViewModel FromMenuItem(MenuItem m) => new MenuItemViewModel
    {
        Id = m.Id,
        MenuID = m.MenuID,
        Name = m.Name,
        Description = m.Description,
        Price = m.Price,
    };
}