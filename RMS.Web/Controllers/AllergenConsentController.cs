using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RMS.Data.Services;
using RMS.Web.Models;

namespace RMS.Web.Controllers;

public class AllergenConsentController : BaseController
{
    private IRestaurantService svc;

    public AllergenConsentController()
    {
        svc = new RestaurantServiceDb();
    }

    // GET /AllergenConsent/Index
    [HttpGet]
    public IActionResult Index()
    {
        var consents = svc.GetAllergenConsents();
        var vms = consents.Select(AllergenConsentViewModel.FromConsent).ToList();
        return View(vms);
    }

    // GET /AllergenConsent/Details/{id}
    [HttpGet]
    public IActionResult Details(int id)
    {
        var consents = svc.GetAllergenConsentsByOrderId(id);

        if (consents is null || consents.Count == 0)
        {
            Alert($"No allergen consents found for order {id}.", AlertType.warning);
            return RedirectToAction(nameof(Index));
        }

        var vms = consents.Select(AllergenConsentViewModel.FromConsent).ToList();
        return View(vms);
    }

    // GET /AllergenConsent/Create
    [HttpGet]
    [Authorize(Roles = "admin,authenticated")]
    public IActionResult Create()
    {
        var vm = new AllergenConsentViewModel();
        vm.AvailableOrders = svc.GetAllOrders();
        vm.AvailableMenuItems = svc.GetAllMenuItems();
        return View(vm);
    }

    // POST /AllergenConsent/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin,authenticated")]
    public IActionResult Create(AllergenConsentViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            vm.AvailableOrders = svc.GetAllOrders();
            vm.AvailableMenuItems = svc.GetAllMenuItems();
            return View(vm);
        }

        var selectedItems = svc.GetAllMenuItems()
            .Where(mi => vm.SelectedMenuItemIds.Contains(mi.Id))
            .ToList();

        var created = svc.AddAllergenConsent(
            vm.OrderId,
            vm.CustomerName,
            vm.CustomerEmail,
            vm.CustomerPhone,
            vm.ConsentGiven,
            selectedItems
        );

        if (created is not null)
        {
            Alert($"Allergen consent recorded for '{created.CustomerName}'.", AlertType.success);
            return RedirectToAction(nameof(Index));
        }

        Alert("Allergen consent could not be recorded.", AlertType.warning);
        vm.AvailableOrders = svc.GetAllOrders();
        vm.AvailableMenuItems = svc.GetAllMenuItems();
        return View(vm);
    }

    // GET /AllergenConsent/Delete/{id}
    [HttpGet]
    [Authorize(Roles = "admin,authenticated")]
    public IActionResult Delete(int id)
    {
        var consents = svc.GetAllergenConsents();
        var consent = consents.FirstOrDefault(c => c.Id == id);

        if (consent is null)
        {
            Alert($"Allergen consent {id} not found.", AlertType.warning);
            return RedirectToAction(nameof(Index));
        }

        return View(AllergenConsentViewModel.FromConsent(consent));
    }

    // POST /AllergenConsent/DeleteConfirm/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin,authenticated")]
    public IActionResult DeleteConfirm(int id)
    {
        svc.DeleteAllergenConsent(id);
        Alert("Allergen consent deleted.", AlertType.success);
        return RedirectToAction(nameof(Index));
    }
}
