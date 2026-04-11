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
    [Authorize(Roles = "admin,owner,manager,staff")]
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
    [Authorize(Roles = "admin,owner,manager,staff")]
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
    [Authorize(Roles = "admin,owner,manager,staff")]
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
    [Authorize(Roles = "admin,owner,manager,staff")]
    public IActionResult MarkCompletedConfirm(int id)
    {
        var updated = svc.MarkOrderCompleted(id);
        Alert(updated is not null ? "Order marked as completed." : "Order could not be updated.",
              updated is not null ? AlertType.success : AlertType.warning);

        return RedirectToAction(nameof(Index));
    }

    // GET /Order/Edit/{id}
    [HttpGet]
    [Authorize(Roles = "admin,owner,manager,staff")]
    public IActionResult Edit(int id)
    {
        var order = svc.GetOrderById(id);

        if (order is null)
        {
            Alert($"Order {id} could not be found.", AlertType.warning);
            return RedirectToAction(nameof(Index));
        }

        var vm = OrderViewModel.FromOrder(order);
        vm.AvailableMenuItems = svc.GetAllMenuItems();
        vm.AvailableTables = svc.GetAllTables();
        vm.SelectedMenuItemIds = order.MenuItems.Select(mi => mi.Id).ToList();
        return View(vm);
    }

    // POST /Order/Edit/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin,owner,manager,staff")]
    public IActionResult Edit(int id, OrderViewModel vm)
    {
        var order = svc.GetOrderById(id);

        if (order is null)
        {
            Alert($"Order {id} could not be found.", AlertType.warning);
            return RedirectToAction(nameof(Index));
        }

        // Update menu items
        order.MenuItems = svc.GetAllMenuItems()
            .Where(mi => vm.SelectedMenuItemIds.Contains(mi.Id))
            .ToList();

        // Update table — if a new table is selected, free the old one and mark the new one occupied
        if (vm.TableId.HasValue)
        {
            var newTable = svc.GetAllTables().FirstOrDefault(t => t.Id == vm.TableId.Value);

            if (order.Table != null && order.Table.Id != vm.TableId.Value)
            {
                order.Table.IsOccupied = false;
            }

            order.Table = newTable;

            if (newTable != null)
            {
                newTable.IsOccupied = true;
            }
        }

        var updated = svc.UpdateOrder(order);

        if (updated is not null)
        {
            Alert("Order updated.", AlertType.success);
            return RedirectToAction(nameof(Details), new { id = updated.Id });
        }

        Alert("Order could not be updated.", AlertType.warning);
        vm.AvailableMenuItems = svc.GetAllMenuItems();
        vm.AvailableTables = svc.GetAllTables();
        return View(vm);
    }

    // GET /Order/Void/{id}
    [HttpGet]
    [Authorize(Roles = "admin,owner")]
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
    [Authorize(Roles = "admin,owner")]
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
    [Authorize(Roles = "admin,owner")]
    public IActionResult DeleteConfirm(int id)
    {
        var deleted = svc.DeleteOrder(id);
        Alert(deleted ? "Order has been deleted." : "Order could not be deleted.",
              deleted ? AlertType.success : AlertType.danger);

        return RedirectToAction(nameof(Index));
    }

    // POST /Order/AdvanceCourse/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin,owner,manager,staff")]
    public IActionResult AdvanceCourse(int id)
    {
        var order = svc.GetOrderById(id);

        if (order is null || order.IsCompleted || order.IsVoid)
        {
            Alert("Order not found or already closed.", AlertType.warning);
            return RedirectToAction(nameof(Details), new { id });
        }

        string nextStatus = order.CourseStatus switch
        {
            "NotStarted"    => "StartersServed",
            "StartersServed" => "MainsServed",
            "MainsServed"   => "DessertsServed",
            _               => order.CourseStatus
        };

        svc.UpdateCourseStatus(id, nextStatus);
        Alert($"Course updated to: {nextStatus.Replace("Served", " Served")}.", AlertType.success);
        return RedirectToAction(nameof(Details), new { id });
    }
}
