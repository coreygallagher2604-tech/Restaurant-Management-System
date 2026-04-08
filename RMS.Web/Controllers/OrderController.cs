using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RMS.Data.Entities;
using RMS.Data.Services;
using RMS.Web.Models;

namespace RMS.Web.Controllers;

public class OrderController : BaseController
{
    private IRestaurantService svc;

    public OrderController()
    {
        svc = new RestaurantServiceDb();
    }

    // GET /Order/Index
    [HttpGet]
    public IActionResult Index()
    {
        var orders = svc.GetAllOrders();
        var vms = orders.Select(OrderViewModel.FromOrder).ToList();
        return View(vms);
    }

    // GET /Order/Details/{id}
    [HttpGet]
    public IActionResult Details(int id)
    {
        var order = svc.GetOrderById(id);

        if (order is null)
        {
            Alert($"Order {id} could not be found.", AlertType.warning);
            return NotFound();
        }
        return View(OrderViewModel.FromOrder(order));
    }

    // GET /Order/Create
    [HttpGet]
    [Authorize(Roles = "admin,authenticated")]
    public IActionResult Create()
    {
        var vm = new OrderViewModel();
        vm.AvailableMenuItems = svc.GetAllMenuItems();
        vm.AvailableTables = svc.GetAllTables();
        return View(vm);
    }

    // POST /Order/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin,authenticated")]
    public IActionResult Create(OrderViewModel vm)
    {
        var selectedItems = svc.GetAllMenuItems()
            .Where(mi => vm.SelectedMenuItemIds.Contains(mi.Id))
            .ToList();

        var created = svc.AddOrder(selectedItems, vm.TableId);

        if (created is not null)
        {
            Alert("Order has been created.", AlertType.success);
            return RedirectToAction(nameof(Details), new { id = created.Id });
        }

        Alert("Order could not be created.", AlertType.warning);
        vm.AvailableMenuItems = svc.GetAllMenuItems();
        vm.AvailableTables = svc.GetAllTables();
        return View(vm);
    }

    // GET /Order/MarkCompleted/{id}
    [HttpGet]
    [Authorize(Roles = "admin,authenticated")]
    public IActionResult MarkCompleted(int id)
    {
        var order = svc.GetOrderById(id);

        if (order is null)
        {
            Alert($"Order {id} could not be found.", AlertType.warning);
            return NotFound();
        }
        return View(OrderViewModel.FromOrder(order));
    }

    // POST /Order/MarkCompletedConfirm
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin,authenticated")]
    public IActionResult MarkCompletedConfirm(int id)
    {
        var updated = svc.MarkOrderCompleted(id);
        Alert(updated is not null ? "Order marked as completed." : "Order could not be updated.",
              updated is not null ? AlertType.success : AlertType.warning);

        return RedirectToAction(nameof(Index));
    }

    // GET /Order/Void/{id}
    [HttpGet]
    [Authorize(Roles = "admin,authenticated")]
    public IActionResult Void(int id)
    {
        var order = svc.GetOrderById(id);

        if (order is null)
        {
            Alert($"Order {id} could not be found.", AlertType.warning);
            return NotFound();
        }
        return View(OrderViewModel.FromOrder(order));
    }

    // POST /Order/VoidConfirm
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin,authenticated")]
    public IActionResult VoidConfirm(int id)
    {
        var updated = svc.VoidOrder(id);
        Alert(updated is not null ? "Order has been voided." : "Order could not be voided.",
              updated is not null ? AlertType.success : AlertType.warning);

        return RedirectToAction(nameof(Index));
    }

    // POST /Order/Delete
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin,authenticated")]
    public IActionResult DeleteConfirm(int id)
    {
        var deleted = svc.DeleteOrder(id);
        Alert(deleted ? "Order has been deleted." : "Order could not be deleted.",
              deleted ? AlertType.success : AlertType.danger);

        return RedirectToAction(nameof(Index));
    }
}
