using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RMS.Data.Entities;
using RMS.Data.Services;
using RMS.Web.Models;

namespace RMS.Web.Controllers;

[Authorize(Roles = "admin,owner,manager,staff")]
public class TableController : BaseController
{
    private IRestaurantService svc;

    public TableController()
    {
        svc = new RestaurantServiceDb();
    }

    // GET /Table/Index
    [HttpGet]
    public IActionResult Index()
    {
        var tables = svc.GetAllTables();
        var vms = tables.Select(TableViewModel.FromTable).ToList();

        // Mark tables reserved if they have a Booked booking within the next 2 hours
        var now = DateTime.Now;
        var horizon = now.AddHours(2);
        var upcomingBookings = svc.GetAllBookings()
            .Where(b => b.Status == "Booked"
                     && b.BookingDateTime >= now
                     && b.BookingDateTime <= horizon)
            .ToList();

        foreach (var vm in vms)
        {
            var match = upcomingBookings.FirstOrDefault(b => b.TableNumber == vm.TableNumber);
            if (match != null)
            {
                vm.IsReserved = true;
                vm.ReservedAt = match.BookingDateTime;
            }
        }

        // Mark occupied tables that have no open order as "Order Needed"
        var openOrders = svc.GetAllOrders()
            .Where(o => !o.IsCompleted && !o.IsVoid)
            .ToList();

        foreach (var vm in vms.Where(v => v.IsOccupied))
        {
            bool hasOpenOrder = openOrders.Any(o => o.Table != null && o.Table.Id == vm.Id);
            if (!hasOpenOrder)
            {
                vm.OrderNeeded = true;
            }
        }

        return View(vms);
    }

    // GET /Table/Create
    [HttpGet]
    [Authorize(Roles = "admin,owner,manager,staff")]
    public IActionResult Create()
    {
        var allTables = svc.GetAllTables();
        int nextTableNumber = allTables.Count > 0 ? allTables.Max(t => t.TableNumber) + 1 : 1;
        return View(new TableViewModel { TableNumber = nextTableNumber });
    }

    // POST /Table/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin,owner,manager,staff")]
    public IActionResult Create(TableViewModel vm)
    {
        // Check for duplicate table number
        bool duplicate = svc.GetAllTables().Any(t => t.TableNumber == vm.TableNumber);
        if (duplicate)
        {
            ModelState.AddModelError("TableNumber", $"Table {vm.TableNumber} already exists.");
        }

        if (ModelState.IsValid)
        {
            var created = svc.AddTable(vm.TableNumber, vm.SeatingCapacity);
            if (created is not null)
            {
                Alert("Table has been added.", AlertType.success);
                return RedirectToAction(nameof(Index));
            }
            Alert("Table could not be added.", AlertType.warning);
        }
        return View(vm);
    }

    // GET /Table/Edit/{id}
    [HttpGet]
    [Authorize(Roles = "admin,owner,manager,staff")]
    public IActionResult Edit(int id)
    {
        var table = svc.GetTableById(id);

        if (table is null)
        {
            Alert($"Table {id} could not be found.", AlertType.warning);
            return NotFound();
        }
        return View(TableViewModel.FromTable(table));
    }

    // POST /Table/Edit/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin,owner,manager,staff")]
    public IActionResult Edit(int id, TableViewModel vm)
    {
        // Check for duplicate table number — exclude the record being edited
        bool duplicate = svc.GetAllTables().Any(t => t.TableNumber == vm.TableNumber && t.Id != id);
        if (duplicate)
        {
            ModelState.AddModelError("TableNumber", $"Table {vm.TableNumber} already exists.");
        }

        if (ModelState.IsValid)
        {
            var updated = svc.UpdateTable(vm.ToTable());
            if (updated is not null)
            {
                Alert("Table has been updated.", AlertType.success);
                return RedirectToAction(nameof(Index));
            }
            Alert("Table could not be updated.", AlertType.warning);
        }
        return View(vm);
    }

    // GET /Table/Delete/{id}
    [HttpGet]
    [Authorize(Roles = "admin,owner,manager,staff")]
    public IActionResult Delete(int id)
    {
        var table = svc.GetTableById(id);

        if (table is null)
        {
            Alert($"Table {id} could not be found.", AlertType.warning);
            return NotFound();
        }
        return View(TableViewModel.FromTable(table));
    }

    // POST /Table/DeleteConfirm
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin,owner,manager")]
    public IActionResult DeleteConfirm(int id)
    {
        var table = svc.GetTableById(id);
        if (table is not null)
        {
            // Block delete if any open (non-void, non-completed) orders are still on this table
            var activeOrders = svc.GetOrdersByTableId(id)
                .Where(o => !o.IsCompleted && !o.IsVoid)
                .ToList();

            if (activeOrders.Count > 0)
            {
                Alert($"Table {table.TableNumber} has {activeOrders.Count} open order(s). Complete or void them first.", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }
        }

        var deleted = svc.DeleteTable(id);
        Alert(deleted ? "Table has been deleted." : "Table could not be deleted.",
              deleted ? AlertType.success : AlertType.danger);

        return RedirectToAction(nameof(Index));
    }

    // GET /Table/SetOccupancy/{id}
    [HttpGet]
    [Authorize(Roles = "admin,owner,manager,staff")]
    public IActionResult SetOccupancy(int id)
    {
        var table = svc.GetTableById(id);

        if (table is null)
        {
            Alert($"Table {id} could not be found.", AlertType.warning);
            return NotFound();
        }
        return View(TableViewModel.FromTable(table));
    }

    // POST /Table/SetOccupancy
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin,owner,manager,staff")]
    public IActionResult SetOccupancy(int id, TableViewModel vm)
    {
        var updated = svc.SetTableOccupancy(id, vm.IsOccupied, vm.CustomersSeated);
        if (updated is not null)
        {
            Alert($"Table {updated.TableNumber} occupancy updated.", AlertType.success);
        }
        else
        {
            Alert("Could not update table occupancy.", AlertType.warning);
        }
        return RedirectToAction(nameof(Index));
    }
}
