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
        // Guests and anonymous users only see active menus
        bool isStaff = User.IsInRole("admin") || User.IsInRole("owner") || User.IsInRole("manager") || User.IsInRole("staff");

        if (isStaff)
        {
            search.Menus = svc.SearchMenus();
        }
        else
        {
            List<Menu> allMenus = svc.SearchMenus().ToList();
            search.Menus = new List<Menu>();
            foreach (Menu m in allMenus)
            {
                if (m.IsActive)
                {
                    search.Menus.Add(m);
                }
            }
        }

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
    [Authorize(Roles = "admin,owner,manager,staff")]
    public IActionResult Create()
    {
        return View(new MenuViewModel());
    }

    // POST /Menu/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin,owner,manager,staff")]
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
    [Authorize(Roles = "admin,owner,manager,staff")]
    public IActionResult Edit(int id)
    {
        var m = svc.GetMenuById(id);

        if (m is null)
        {
            Alert($"Menu {id} Has Not Been Found.", AlertType.warning);
            return NotFound();
        }
        var vm = MenuViewModel.FromMenu(m);
        var currentIds = vm.MenuItems.Select(i => i.Id).ToHashSet();
        vm.AvailableMenuItems = svc.GetAllMenuItems()
            .Where(i => !currentIds.Contains(i.Id))
            .ToList();
        return View(vm);
    }

    // POST /Menu/Edit/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin,owner,manager,staff")]
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

    // POST /Menu/AddItemToMenu
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin,owner,manager,staff")]
    public IActionResult AddItemToMenu(int menuId, int menuItemId)
    {
        svc.AddMenuItemToMenu(menuId, menuItemId);
        return RedirectToAction(nameof(Edit), new { id = menuId });
    }

    // POST /Menu/RemoveItemFromMenu
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin,owner,manager,staff")]
    public IActionResult RemoveItemFromMenu(int menuId, int menuItemId)
    {
        svc.RemoveMenuItemFromMenu(menuId, menuItemId);
        return RedirectToAction(nameof(Edit), new { id = menuId });
    }

    // GET /Menu/Delete/{id}
    [HttpGet]
    [Authorize(Roles = "admin,owner,manager,staff")]
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
    [Authorize(Roles = "admin,owner,manager,staff")]
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

    [HttpGet]
    [Authorize(Roles = "admin,owner,manager,staff")]
    public IActionResult MenuItems(MenuItemSearchViewModel search)
    {
        var items = svc.GetAllMenuItems();
        search.MenuItems = string.IsNullOrWhiteSpace(search.Query)
            ? items
            : items.Where(i => i.Name.Contains(search.Query, StringComparison.OrdinalIgnoreCase)).ToList();
        return View(search);
    }

    // GET /Menu/AddMenuItem
    [HttpGet]
    [Authorize(Roles = "admin,owner,manager,staff")]
    public IActionResult AddMenuItem()
    {
        return View(new MenuItemViewModel());
    }

    // POST /Menu/AddMenuItem
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin,owner,manager,staff")]
    public IActionResult AddMenuItem(MenuItemViewModel vm)
    {
        if (ModelState.IsValid)
        {
            var item = svc.AddMenuItem(vm.Name, vm.Type, vm.Description, vm.Price, new List<Ingredient>());
            if (item is not null)
            {
                Alert("Menu Item Has Been Added.", AlertType.success);
                return RedirectToAction(nameof(MenuItems));
            }
            Alert("Menu Item could not be added.", AlertType.warning);
        }
        return View(vm);
    }

    // GET /Menu/MenuItemDetails/{id}
    [HttpGet]
    public IActionResult MenuItemDetails(int id)
    {
        var item = svc.GetMenuItemById(id);

        if (item is null)
        {
            Alert($"Menu Item {id} Has Not Been Found.", AlertType.warning);
            return NotFound();
        }
        return View(MenuItemViewModel.FromMenuItem(item));
    }

    // GET /Menu/EditMenuItem/{id}
    [HttpGet]
    [Authorize(Roles = "admin,owner,manager,staff")]
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
    [Authorize(Roles = "admin,owner,manager,staff")]
    public IActionResult EditMenuItem(int id, MenuItemViewModel vm)
    {
        if (ModelState.IsValid)
        {
            var existing = svc.GetMenuItemById(id);
            var menuItem = vm.ToMenuItem();
            menuItem.Ingredients = existing.Ingredients;        
            var updated = svc.UpdateMenuItem(menuItem);
            
            if (updated is not null)
            {
                Alert("Menu Item Has Been Updated.", AlertType.success);
                return RedirectToAction(nameof(MenuItems));
            }
            Alert("Menu Item could not be updated.", AlertType.warning);
        }
        return View(vm);
    }

    // GET /Menu/DeleteMenuItem/{id}
    [HttpGet]
    [Authorize(Roles = "admin,owner,manager,staff")]
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
    [Authorize(Roles = "admin,owner,manager,staff")]
    public IActionResult DeleteMenuItemConfirm(int id)
    {
        var deleted = svc.DeleteMenuItem(id);
        Alert(deleted ? "Menu Item has been deleted." : "Menu Item could not be deleted.",
              deleted ? AlertType.success : AlertType.danger);

        return RedirectToAction(nameof(MenuItems));
    }
}



