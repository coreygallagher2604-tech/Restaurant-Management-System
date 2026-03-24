using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RMS.Data.Entities;
using RMS.Data.Services;
using RMS.Web.Models;

namespace RMS.Web.Controllers;

public class MenuController : BaseController
{
    private IRestaurantService svc;

    public MenuController()
    {
        svc = new RestaurantServiceDb();
    }

    // GET /Menu/Index
    [HttpGet]
    public IActionResult Index(string orderBy = "id", string direction = "asc")
    {
        var data = svc.GetAllMenus(orderBy, direction);
        return View(data);
    }

    // GET /Menu/Menus
    [HttpGet]
    public IActionResult Menus(MenuSearchViewModel search)
    {
        search.Menus = svc.SearchMenus();

        if (Request.Headers.ContainsKey("HX-Request"))
        {
            return PartialView("Menus", search);
        }
        return View("Menus", search);
    }

    // GET /Menu/Details/{id}
    [HttpGet]
    public IActionResult Details(int id)
    {
        var m = svc.GetMenuById(id);

        if (m is null)
        {
            Alert($"Menu {id} Has Not Been Found.", AlertType.warning);
            return NotFound();
        }
        return View(MenuViewModel.FromMenu(m));
    }

    // GET /Menu/Create
    [HttpGet]
    [Authorize(Roles = "admin,authenticated")]
    public IActionResult Create()
    {
        return View(new MenuViewModel());
    }

    // POST /Menu/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin,authenticated")]
    public IActionResult Create(MenuViewModel vm)
    {
        if (ModelState.IsValid)
        {
            var created = svc.AddMenu(vm.Name, vm.Type, vm.Description, vm.IsActive, vm.MenuItems.ToList());
            if (created is not null)
            {
                Alert("Menu Has Been Created.", AlertType.success);
                return RedirectToAction(nameof(Details), new { Id = created.Id });
            }
            Alert("Menu could not be created as the name already exists.", AlertType.warning);
        }
        return View(vm);
    }

    // GET /Menu/Edit/{id}
    [HttpGet]
    [Authorize(Roles = "admin,authenticated")]
    public IActionResult Edit(int id)
    {
        var m = svc.GetMenuById(id);

        if (m is null)
        {
            Alert($"Menu {id} Has Not Been Found.", AlertType.warning);
            return NotFound();
        }
        return View(MenuViewModel.FromMenu(m));
    }

    // POST /Menu/Edit/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin,authenticated")]
    public IActionResult Edit(int id, MenuViewModel vm)
    {
        if (ModelState.IsValid)
        {
            var updated = svc.UpdateMenu(vm.ToMenu());
            if (updated is not null)
            {
                Alert("Menu Has Been Updated.", AlertType.success);
                return RedirectToAction(nameof(Details), new { Id = updated.Id });
            }
            Alert("Menu could not be updated.", AlertType.warning);
        }
        return View(vm);
    }

    // GET /Menu/Delete/{id}
    [HttpGet]
    [Authorize(Roles = "admin,authenticated")]
    public IActionResult Delete(int id)
    {
        var m = svc.GetMenuById(id);

        if (m is null)
        {
            Alert($"Menu {id} Could Not Be Found.", AlertType.warning);
            return NotFound();
        }
        return View(MenuViewModel.FromMenu(m));
    }

    // POST /Menu/DeleteConfirm
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin,authenticated")]
    public IActionResult DeleteConfirm(int id)
    {
        var m = svc.GetMenuById(id);

        if (m is null)
        {
            Alert("Menu could not be found.", AlertType.warning);
            return RedirectToAction(nameof(Index));
        }

        var deleted = svc.DeleteMenu(id);
        Alert(deleted ? "Menu has been deleted." : "Menu could not be deleted.",
              deleted ? AlertType.success : AlertType.danger);

        return RedirectToAction(nameof(Index));
    }

    // ===================== MENU ITEM ACTIONS =====================

    // GET /Menu/AddMenuItem/{menuId}
    [HttpGet]
    [Authorize(Roles = "admin,authenticated")]
    public IActionResult AddMenuItem(int menuId)
    {
        var m = svc.GetMenuById(menuId);

        if (m is null)
        {
            Alert($"Menu {menuId} Could Not Be Found.", AlertType.warning);
            return NotFound();
        }
        return View(new MenuItemViewModel { MenuID = menuId });
    }

    // POST /Menu/AddMenuItem
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin,authenticated")]
    public IActionResult AddMenuItem(MenuItemViewModel vm)
    {
        if (ModelState.IsValid)
        {
            var item = svc.AddMenuItem(vm.Name, vm.Description, vm.Price, new List<Ingredient>());
            if (item is not null)
            {
                Alert("Menu Item Has Been Added.", AlertType.success);
                return RedirectToAction(nameof(Details), new { Id = vm.MenuID });
            }
            Alert("Menu Item could not be added.", AlertType.warning);
        }
        return View(vm);
    }

    // GET /Menu/EditMenuItem/{id}
    [HttpGet]
    [Authorize(Roles = "admin,authenticated")]
    public IActionResult EditMenuItem(int id)
    {
        var item = svc.GetMenuItemById(id);

        if (item is null)
        {
            Alert($"Menu Item {id} Could Not Be Found.", AlertType.warning);
            return NotFound();
        }
        return View(MenuItemViewModel.FromMenuItem(item));
    }

    // POST /Menu/EditMenuItem/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin,authenticated")]
    public IActionResult EditMenuItem(int id, MenuItemViewModel vm)
    {
        if (ModelState.IsValid)
        {
            var updated = svc.UpdateMenuItem(vm.ToMenuItem());
            if (updated is not null)
            {
                Alert("Menu Item Has Been Updated.", AlertType.success);
                return RedirectToAction(nameof(Details), new { Id = vm.MenuID });
            }
            Alert("Menu Item could not be updated.", AlertType.warning);
        }
        return View(vm);
    }

    // GET /Menu/DeleteMenuItem/{id}
    [HttpGet]
    [Authorize(Roles = "admin,authenticated")]
    public IActionResult DeleteMenuItem(int id)
    {
        var item = svc.GetMenuItemById(id);

        if (item is null)
        {
            Alert($"Menu Item {id} Could Not Be Found.", AlertType.warning);
            return NotFound();
        }
        return View(MenuItemViewModel.FromMenuItem(item));
    }

    // POST /Menu/DeleteMenuItemConfirm
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin,authenticated")]
    public IActionResult DeleteMenuItemConfirm(int id, int menuId)
    {
        var deleted = svc.DeleteMenuItem(id);
        Alert(deleted ? "Menu Item has been deleted." : "Menu Item could not be deleted.",
              deleted ? AlertType.success : AlertType.danger);

        return RedirectToAction(nameof(Details), new { Id = menuId });
    }
}



