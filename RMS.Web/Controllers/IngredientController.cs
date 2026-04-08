using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RMS.Data.Entities;
using RMS.Data.Services;
using RMS.Web.Models;

namespace RMS.Web.Controllers;

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
    [Authorize(Roles = "admin,authenticated")]
    public IActionResult Create()
    {
        return View(new IngredientViewModel());
    }

    // POST /Ingredient/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin,authenticated")]
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
    [Authorize(Roles = "admin,authenticated")]
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

    // POST /Ingredient/Edit/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin,authenticated")]
    public IActionResult Edit(int id, IngredientViewModel vm)
    {
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
    [Authorize(Roles = "admin,authenticated")]
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
    [Authorize(Roles = "admin,authenticated")]
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
