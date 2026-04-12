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

    public List<Ingredient> AvailableIngredients { get; set; } = new();

    public List<int> SelectedIngredientIds { get; set; } = new();

    public MenuItem ToMenuItem()
    {
        var item = new MenuItem();
        item.Id = Id;
        item.Name = Name;
        item.Type = Type;
        item.Description = Description;
        item.Price = Price;
        return item;
    }

    public static MenuItemViewModel FromMenuItem(MenuItem m)
    {
        var vm = new MenuItemViewModel();
        vm.Id = m.Id;
        vm.Name = m.Name;
        vm.Type = m.Type;
        vm.Description = m.Description;
        vm.Price = m.Price;
        vm.Ingredients = m.Ingredients ?? new();
        vm.SelectedIngredientIds = m.Ingredients?.Select(i => i.Id).ToList() ?? new();
        return vm;
    }
}