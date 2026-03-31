using System.ComponentModel.DataAnnotations;
using RMS.Data.Entities;

namespace RMS.Web.Models;

public class MenuItemViewModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; set; }

    [Required]
    public string Type { get; set; }

    [StringLength(500)]
    public string Description { get; set; }

    [Required]
    [Range(0.01, 10000)]
    [DisplayFormat(DataFormatString = "{0:F2}")]
    public double Price { get; set; }

    public List<Ingredient> Ingredients { get; set; } = new();

    public MenuItem ToMenuItem() => new MenuItem
    {
        Id = this.Id,
        Name = this.Name,
        Type = this.Type,
        Description = this.Description,
        Price = this.Price,
    };

    public static MenuItemViewModel FromMenuItem(MenuItem m) => new MenuItemViewModel
    {
        Id = m.Id,
        Name = m.Name,
        Type = m.Type,
        Description = m.Description,
        Price = m.Price,
        Ingredients = m.Ingredients ?? new(),
    };
}