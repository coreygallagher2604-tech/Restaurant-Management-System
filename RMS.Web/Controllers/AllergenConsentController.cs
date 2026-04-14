using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RMS.Data.Entities;
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

    // GET /AllergenConsent/Create?orderId=0
    // orderId is optional — when the user picks an order from the dropdown the page
    // reloads with that order's menu items shown in the checkbox list
    [HttpGet]
    [Authorize(Roles = "admin,owner,manager,staff")]
    public IActionResult Create(int orderId = 0)
    {
        var vm = new AllergenConsentViewModel();
        vm.AvailableOrders = svc.GetAllOrders().Where(o => !o.IsCompleted && !o.IsVoid).ToList();
        vm.OrderId = orderId;

        if (orderId > 0)
        {
            var order = svc.GetOrderById(orderId);
            vm.AvailableMenuItems = order?.MenuItems ?? new List<MenuItem>();
        }

        return View(vm);
    }

    // POST /AllergenConsent/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin,owner,manager,staff")]
    public IActionResult Create(AllergenConsentViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            vm.AvailableOrders = svc.GetAllOrders().Where(o => !o.IsCompleted && !o.IsVoid).ToList();
            if (vm.OrderId > 0)
            {
                var order = svc.GetOrderById(vm.OrderId);
                vm.AvailableMenuItems = order?.MenuItems ?? new List<MenuItem>();
            }
            return View(vm);
        }

        var selectedItems = vm.OrderId > 0
            ? (svc.GetOrderById(vm.OrderId)?.MenuItems ?? new List<MenuItem>())
                .Where(mi => vm.SelectedMenuItemIds.Contains(mi.Id)).ToList()
            : new List<MenuItem>();

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
        vm.AvailableOrders = svc.GetAllOrders().Where(o => !o.IsCompleted && !o.IsVoid).ToList();
        return View(vm);
    }

    // GET /AllergenConsent/Edit/{id}
    [HttpGet]
    [Authorize(Roles = "admin,owner,manager,staff")]
    public IActionResult Edit(int id)
    {
        var consents = svc.GetAllergenConsents();
        var consent = consents.FirstOrDefault(c => c.Id == id);

        if (consent is null)
        {
            Alert($"Allergen consent {id} not found.", AlertType.warning);
            return RedirectToAction(nameof(Index));
        }

        var vm = AllergenConsentViewModel.FromConsent(consent);
        vm.AvailableMenuItems = svc.GetAllMenuItems();
        vm.SelectedMenuItemIds = consent.MenuItems.Select(m => m.Id).ToList();
        return View(vm);
    }

    // POST /AllergenConsent/Edit/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin,owner,manager,staff")]
    public IActionResult Edit(int id, AllergenConsentViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            vm.AvailableMenuItems = svc.GetAllMenuItems();
            return View(vm);
        }

        svc.UpdateAllergenConsent(id, vm.CustomerName, vm.CustomerEmail, vm.CustomerPhone, vm.ConsentGiven);
        Alert($"Allergen consent for '{vm.CustomerName}' updated.", AlertType.success);
        return RedirectToAction(nameof(Index));
    }

    // GET /AllergenConsent/Delete/{id}
    [HttpGet]
    [Authorize(Roles = "admin,owner,manager,staff")]
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
    [Authorize(Roles = "admin,owner,manager,staff")]
    public IActionResult DeleteConfirm(int id)
    {
        svc.DeleteAllergenConsent(id);
        Alert("Allergen consent deleted.", AlertType.success);
        return RedirectToAction(nameof(Index));
    }
}
