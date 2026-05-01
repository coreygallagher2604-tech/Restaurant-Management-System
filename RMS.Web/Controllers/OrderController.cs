using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RMS.Data.Entities;
using RMS.Data.Services;
using RMS.Web.Models;

namespace RMS.Web.Controllers;

[Authorize(Roles = "admin,owner,manager,staff")]
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
        var vms = orders
            .OrderBy(o => o.IsCompleted || o.IsVoid ? 1 : 0)
            .ThenByDescending(o => o.CreatedOn)
            .Select(OrderViewModel.FromOrder)
            .ToList();
        return View(vms);
    }

    // GET /Order/ByTable/{tableId} — active orders for a specific table
    [HttpGet]
    [Authorize(Roles = "admin,owner,manager,staff")]
    public IActionResult ByTable(int tableId)
    {
        var table = svc.GetTableById(tableId);
        if (table is null)
        {
            Alert("Table not found.", AlertType.warning);
            return RedirectToAction(nameof(Index));
        }
        var orders = svc.GetOrdersByTableId(tableId)
                        .Where(o => !o.IsCompleted && !o.IsVoid)
                        .ToList();
        ViewBag.TableNumber = table.TableNumber;
        return View("Index", orders.Select(OrderViewModel.FromOrder).ToList());
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
        var quantities = (vm.ItemQuantities ?? new Dictionary<int, int>())
            .Where(kvp => kvp.Value > 0)
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        var created = svc.AddOrder(quantities, vm.TableId);

        if (created is not null)
        {
            if (created.AllergyCountRequired > 0)
            {
                Alert($"Order created. This table has {created.AllergyCountRequired} customer(s) who declared an allergy — allergen consent form(s) must be completed before the order can be closed.", AlertType.warning);
                return RedirectToAction("Create", "AllergenConsent", new { orderId = created.Id });
            }

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

        var consentCount = order.AllergenConsents?.Count ?? 0;
        if (order.AllergyCountRequired > 0 && consentCount < order.AllergyCountRequired)
        {
            Alert($"This order cannot be completed. {order.AllergyCountRequired} allergen consent form(s) required — {consentCount} recorded. Please add the missing consent(s) first.", AlertType.warning);
            return RedirectToAction(nameof(Details), new { id });
        }

        return View(OrderViewModel.FromOrder(order));
    }

    // POST /Order/MarkCompletedConfirm
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin,owner,manager,staff")]
    public IActionResult MarkCompletedConfirm(int id)
    {
        var order = svc.GetOrderById(id);
        if (order is not null)
        {
            var consentCount = order.AllergenConsents?.Count ?? 0;
            if (order.AllergyCountRequired > 0 && consentCount < order.AllergyCountRequired)
            {
                Alert($"This order cannot be completed. {order.AllergyCountRequired} allergen consent form(s) required — {consentCount} recorded.", AlertType.warning);
                return RedirectToAction(nameof(Details), new { id });
            }
        }

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

        // Update order items from quantities
        var allMenuItems = svc.GetAllMenuItems().ToDictionary(mi => mi.Id);
        order.OrderItems = (vm.ItemQuantities ?? new Dictionary<int, int>())
            .Where(kvp => kvp.Value > 0 && allMenuItems.ContainsKey(kvp.Key))
            .Select(kvp => new OrderItem
            {
                MenuItemId = kvp.Key,
                Quantity = kvp.Value,
                UnitPrice = allMenuItems[kvp.Key].Price
            })
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

    // POST /Order/RestoreConfirm — undo a void, sets IsVoid back to false
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin,owner")]
    public IActionResult RestoreConfirm(int id)
    {
        var updated = svc.VoidOrder(id, false);
        Alert(updated is not null ? "Order has been restored." : "Order could not be restored.",
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
