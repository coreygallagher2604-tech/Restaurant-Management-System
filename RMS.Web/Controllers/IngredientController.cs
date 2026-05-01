using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RMS.Data.Entities;
using RMS.Data.Services;
using RMS.Web.Models;

namespace RMS.Web.Controllers;

[Authorize(Roles = "admin,owner,manager")]
public class IngredientController : BaseController
{
    private IRestaurantService svc;

    public IngredientController()
    {
        svc = new RestaurantServiceDb();
    }

    // GET /Ingredient/Index
    [HttpGet]
    public IActionResult Index()
    {
        var ingredients = svc.GetAllIngredients();
        var vms = ingredients.Select(IngredientViewModel.FromIngredient).ToList();
        return View(vms);
    }

    // GET /Ingredient/Create
    [HttpGet]
    [Authorize(Roles = "admin,owner,manager")]
    public IActionResult Create()
    {
        return View(new IngredientViewModel());
    }

    // POST /Ingredient/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin,owner,manager")]
    public IActionResult Create(IngredientViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        var created = svc.AddIngredient(vm.Name, vm.Allergen, vm.AllergenInfo);

        if (created is not null)
        {
            Alert($"Ingredient '{created.Name}' added.", AlertType.success);
            return RedirectToAction(nameof(Index));
        }

        Alert("Ingredient could not be added.", AlertType.warning);
        return View(vm);
    }

    // GET /Ingredient/Edit/{id}
    [HttpGet]
    [Authorize(Roles = "admin,owner,manager")]
    public IActionResult Edit(int id)
    {
        var ingredient = svc.GetIngredientById(id);

        if (ingredient is null)
        {
            Alert($"Ingredient {id} not found.", AlertType.warning);
            return RedirectToAction(nameof(Index));
        }

        return View(IngredientViewModel.FromIngredient(ingredient));
    }

    // The 14 allergens with protected status under EU food law — allergen flag cannot be removed
    private static readonly string[] ProtectedAllergens =
    {
        "Milk", "Eggs", "Gluten", "Crustaceans", "Fish", "Tree Nuts",
        "Sulfites", "Mustard", "Celery", "Soya", "Peanuts", "Lupin", "Molluscs", "Sesame"
    };

    // POST /Ingredient/Edit/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin,owner,manager")]
    public IActionResult Edit(int id, IngredientViewModel vm)
    {
        // Prevent removing allergen status from any of the 14 legally protected allergens
        if (ProtectedAllergens.Contains(vm.Name) && !vm.Allergen)
        {
            ModelState.AddModelError(nameof(vm.Allergen),
                $"'{vm.Name}' is one of the 14 EU-listed allergens. Its allergen status cannot be removed.");
        }

        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        var ingredient = vm.ToIngredient();
        ingredient.Id = id;
        var updated = svc.UpdateIngredient(ingredient);

        if (updated is not null)
        {
            Alert($"Ingredient '{updated.Name}' updated.", AlertType.success);
            return RedirectToAction(nameof(Index));
        }

        Alert("Ingredient could not be updated.", AlertType.warning);
        return View(vm);
    }

    // GET /Ingredient/Delete/{id}
    [HttpGet]
    [Authorize(Roles = "admin,owner,manager")]
    public IActionResult Delete(int id)
    {
        var ingredient = svc.GetIngredientById(id);

        if (ingredient is null)
        {
            Alert($"Ingredient {id} not found.", AlertType.warning);
            return RedirectToAction(nameof(Index));
        }

        return View(IngredientViewModel.FromIngredient(ingredient));
    }

    // POST /Ingredient/DeleteConfirm/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin,owner,manager")]
    public IActionResult DeleteConfirm(int id)
    {
        var deleted = svc.DeleteIngredient(id);

        if (deleted)
        {
            Alert("Ingredient deleted.", AlertType.success);
        }
        else
        {
            Alert("Ingredient could not be deleted.", AlertType.warning);
        }

        return RedirectToAction(nameof(Index));
    }
}
