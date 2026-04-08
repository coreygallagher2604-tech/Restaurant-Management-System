using System.ComponentModel.DataAnnotations;
using RMS.Data.Entities;

namespace RMS.Web.Models;

public class IngredientViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    public string Name { get; set; } = "";

    public bool Allergen { get; set; } = false;

    public string AllergenInfo { get; set; } = "No known allergen";

    public static IngredientViewModel FromIngredient(Ingredient i)
    {
        var vm = new IngredientViewModel();
        vm.Id = i.Id;
        vm.Name = i.Name;
        vm.Allergen = i.Allergen;
        vm.AllergenInfo = i.AllergenInfo;
        return vm;
    }

    public Ingredient ToIngredient()
    {
        var i = new Ingredient();
        i.Id = Id;
        i.Name = Name;
        i.Allergen = Allergen;
        i.AllergenInfo = AllergenInfo;
        return i;
    }
}
