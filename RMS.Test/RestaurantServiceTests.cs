
using System;
using System.Linq;
using Xunit;

using RMS.Data.Services;
using RMS.Data.Entities;
using Microsoft.VisualBasic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using RMS.Data.Repository;
using System.Diagnostics.Metrics;

namespace RMS.Test;
   

// ==================== AllergenConsent Tests =============================
[Collection("Sequential")]
public class AllergenConsentServiceTests
{
    


    private readonly IRestaurantService svc;

    public AllergenConsentServiceTests()
    {
        // general arrangement
        svc = new RestaurantServiceDb();
        
        // ensure data source is empty before each test
        svc.Initialise();
    }


    // ==================== Add Allergen Consent Tests =============================


    [Fact]

     public void Add_Allergen_Consent()

    {
        //arrange
        int orderId = 1;
        string customerName = "John Doe";
        string customerEmail = "john.doe@example.com";
        string customerPhone = "123-456-7890";
        bool consentGiven = true;

        var ingredient1 = svc.AddIngredient("Peanuts", true, "Contains peanuts");
        var ingredient2 = svc.AddIngredient("Milk", true, "Contains milk");
        var ingredient3 = svc.AddIngredient("Eggs", true, "Contains eggs");
        
        var menuItem1 = svc.AddMenuItem("Peanut Butter Brownie", "A brownie mixed with peanut butter.", 5.99, new List<Ingredient> { ingredient1 });
        var menuItem2 = svc.AddMenuItem("Milkshake", "A creamy milkshake made with real milk.", 3.99, new List<Ingredient> { ingredient2 });
        var menuItem3 = svc.AddMenuItem("Omelette", "A fluffy omelette made with fresh eggs.", 4.99, new List<Ingredient> { ingredient3 });

        List<MenuItem> menuItems = new List<MenuItem> { menuItem1, menuItem2, menuItem3 };

        //act
        var consent = svc.AddAllergenConsent(orderId, customerName, customerEmail, customerPhone, consentGiven, menuItems);

        //assert
        Assert.NotNull(consent);
        Assert.Equal(orderId, consent.OrderId);
        Assert.Equal(customerName, consent.CustomerName);
        Assert.Equal(customerEmail, consent.CustomerEmail);
        Assert.Equal(customerPhone, consent.CustomerPhone);
        Assert.Equal(consentGiven, consent.ConsentGiven);
        Assert.Equal(menuItems.Count, consent.MenuItems.Count);
        for (int i = 0; i < menuItems.Count; i++)
        {
            Assert.Equal(menuItems[i].Id, consent.MenuItems[i].Id);
            Assert.Equal(menuItems[i].Name, consent.MenuItems[i].Name);
            Assert.Equal(menuItems[i].Description, consent.MenuItems[i].Description);
            Assert.Equal(menuItems[i].Price, consent.MenuItems[i].Price);
        }



}


 // ==================== Get Allergen Consent Tests =============================

    [Fact]
    public void Get_Allergen_Consents()
    {
        //arrange
        int orderId = 1;
        string customerName = "John Doe";
        string customerEmail = "john.doe@example.com";
        string customerPhone = "123-456-7890";
        bool consentGiven = true;

        var ingredient1 = svc.AddIngredient("Peanuts", true, "Contains peanuts");
        var ingredient2 = svc.AddIngredient("Milk", true, "Contains milk");
        var ingredient3 = svc.AddIngredient("Eggs", true, "Contains eggs");
        
        var menuItem1 = svc.AddMenuItem("Peanut Butter Brownie", "A brownie mixed with peanut butter.", 5.99, new List<Ingredient> { ingredient1 });
        var menuItem2 = svc.AddMenuItem("Milkshake", "A creamy milkshake made with real milk.", 3.99, new List<Ingredient> { ingredient2 });
        var menuItem3 = svc.AddMenuItem("Omelette", "A fluffy omelette made with fresh eggs.", 4.99, new List<Ingredient> { ingredient3 });

        List<MenuItem> menuItems = new List<MenuItem> { menuItem1, menuItem2, menuItem3 };
        var consent = svc.AddAllergenConsent(orderId, customerName, customerEmail, customerPhone, consentGiven, menuItems);
        
        //act
        var consents = svc.GetAllergenConsents();
        //assert
        Assert.NotNull(consents);
        Assert.Equal(orderId, consents[0].OrderId);
        Assert.Equal(customerName, consents[0].CustomerName);
        Assert.Equal(customerEmail, consents[0].CustomerEmail);
        Assert.Equal(customerPhone, consents[0].CustomerPhone);
        Assert.Equal(consentGiven, consents[0].ConsentGiven);
        Assert.Equal(menuItems.Count, consents[0].MenuItems.Count);
        for (int i = 0; i < menuItems.Count; i++)
        {
            Assert.Equal(menuItems[i].Id, consents[0].MenuItems[i].Id);
            Assert.Equal(menuItems[i].Name, consents[0].MenuItems[i].Name);
            Assert.Equal(menuItems[i].Description, consents[0].MenuItems[i].Description);
            Assert.Equal(menuItems[i].Price, consents[0].MenuItems[i].Price);
        }





    }

}













// ==================== MenuService Tests =============================


[Collection("Sequential")]
public class MenuServiceTests
{
    private readonly IRestaurantService svc;

    public MenuServiceTests()
    {
        // general arrangement
        svc = new RestaurantServiceDb();
        
        // ensure data source is empty before each test
        svc.Initialise();
    }

// ==================== Add Menu Tests =============================

     //Add Menu Test
    [Fact]   
    public void Add_Menu_If_Does_not_exist()
    {
        DateTime year = DateTime.ParseExact("03/14/1973", "mm/dd/yyyy", CultureInfo.InvariantCulture);

        var ingredient1 = svc.AddIngredient("Chicken Breast");
        var ingredient2 = svc.AddIngredient("Lettuce");
        var ingredient3 = svc.AddIngredient("Tomato");
        var ingredient4 = svc.AddIngredient("Basil");
        var ingredient5 = svc.AddIngredient("Garlic");
        var ingredient6= svc.AddIngredient("Peanuts", true, "Contains peanuts");

        List<Ingredient> ingredients = new List<Ingredient> { ingredient1, ingredient2, ingredient3, ingredient4, ingredient5, ingredient6 };

        MenuItem menuItem1 = svc.AddMenuItem("Chicken Salad", "Fresh chicken salad with lettuce, tomato and basil.", 9.99, ingredients);

        List<MenuItem> menuItems = new List<MenuItem> { menuItem1 };
        
        //arrange - add new Menu
        Menu menu = svc.AddMenu("Lunch Menu", "Lunch", "Lunch selection with starters, mains and desserts.", true, menuItems);
        //act
        // Check if Menu exists with same title
        var exists = svc.GetMenuByName("Lunch Menu");

        //assert Menu is not null
        Assert.Equal(menu, exists);

    }



// ==================== Get Menu Tests =============================    

   
   
   
   
   // ==================== Get Menu By ID Test =============================   

   [Fact]
    public void Can_get_Menu_by_ID()
    {
       // arrange
       var ingredient = svc.AddIngredient("Chicken");
       var menuItem = svc.AddMenuItem("Chicken Curry", "Spicy curry", 12.50, new List<Ingredient> { ingredient });
       var created = svc.AddMenu("Dinner Menu", "Dinner", "Evening menu", true, new List<MenuItem> { menuItem });

       // act
       var found = svc.GetMenuById(created.Id);

       // assert
       Assert.NotNull(found);
       Assert.Equal(created.Id, found.Id);
       Assert.Equal("Dinner Menu", found.Name);
       Assert.Equal("Dinner", found.Type);
       Assert.Equal("Evening menu", found.Description);
       Assert.True(found.IsActive);
       Assert.Single(found.MenuItems);
       Assert.Equal("Chicken Curry", found.MenuItems[0].Name);
    }

        
        // ==================== Get Menu By Name Test =============================   
    [Fact]
    public void Can_get_Menu_by_name()
    {
        // arrange
        var ingredient = svc.AddIngredient("Tomato");
        var menuItem = svc.AddMenuItem("Tomato Soup", "Classic soup", 5.25, new List<Ingredient> { ingredient });
        var created = svc.AddMenu("Lunch Specials", "Lunch", "Daily lunch specials", true, new List<MenuItem> { menuItem });

        // act
        var found = svc.GetMenuByName("Lunch Specials");

        // assert
        Assert.NotNull(found);
        Assert.Equal(created.Id, found.Id);
        Assert.Equal("Lunch Specials", found.Name);
        Assert.Equal("Lunch", found.Type);
        Assert.Equal("Daily lunch specials", found.Description);
        Assert.True(found.IsActive);

    }


    // ==================== Menu Search Tests =============================



        // ==================== Search 1 Menu Test =============================   
    [Fact]
    public void Search_Menus_Only_1_Exists()
    {
        // arrange
        var ingredient = svc.AddIngredient("Beef");
        var menuItem = svc.AddMenuItem("Beef Burger", "Chargrilled burger", 10.99, new List<Ingredient> { ingredient });
        var created = svc.AddMenu("Main Menu", "All Day", "Core menu", true, new List<MenuItem> { menuItem });

        // act
        var menus = svc.SearchMenus();

        // assert
        Assert.NotNull(menus);
        Assert.Single(menus);
        Assert.Equal(created.Id, menus[0].Id);
        Assert.Equal("Main Menu", menus[0].Name);
    }

       // ==================== Search Multiple Menu Test =============================   

    [Fact]
    public void Search_Menus_Multiple_Exist()
    {
        // arrange
        var ingredient1 = svc.AddIngredient("Pasta");
        var ingredient2 = svc.AddIngredient("Rice");

        var item1 = svc.AddMenuItem("Pasta Bake", "Creamy pasta", 11.00, new List<Ingredient> { ingredient1 });
        var item2 = svc.AddMenuItem("Chicken Rice", "Steamed rice and chicken", 10.00, new List<Ingredient> { ingredient2 });

        var menu1 = svc.AddMenu("Menu A", "Lunch", "First menu", true, new List<MenuItem> { item1 });
        var menu2 = svc.AddMenu("Menu B", "Dinner", "Second menu", true, new List<MenuItem> { item2 });

        // act
        var menus = svc.SearchMenus();

        // assert
        Assert.NotNull(menus);
        Assert.Equal(2, menus.Count);
        Assert.Equal(menu1.Id, menus[0].Id);
        Assert.Equal(menu2.Id, menus[1].Id);
    }


      
        // ==================== Update Menu Tests =============================

    [Fact]
    public void Update_Menu()
    {
        // arrange
        var ingredient1 = svc.AddIngredient("Salmon");
        var ingredient2 = svc.AddIngredient("Lemon");
        var oldItem = svc.AddMenuItem("Grilled Salmon", "Original description", 14.99, new List<Ingredient> { ingredient1 });
        var newItem = svc.AddMenuItem("Lemon Salmon", "Updated dish", 15.99, new List<Ingredient> { ingredient1, ingredient2 });

        var created = svc.AddMenu("Seafood Menu", "Dinner", "Original menu description", true, new List<MenuItem> { oldItem });

        var updatedMenu = new Menu
        {
            Id = created.Id,
            Name = "Updated Seafood Menu",
            Type = "Evening",
            Description = "Updated menu description",
            IsActive = false,
            MenuItems = new List<MenuItem> { newItem }
        };

        // act
        var result = svc.UpdateMenu(updatedMenu);
        var reloaded = svc.GetMenuById(created.Id);

        // assert
        Assert.NotNull(result);
        Assert.NotNull(reloaded);
        Assert.Equal(created.Id, result.Id);
        Assert.Equal("Updated Seafood Menu", result.Name);
        Assert.Equal("Evening", result.Type);
        Assert.Equal("Updated menu description", result.Description);
        Assert.False(result.IsActive);
        Assert.Single(result.MenuItems);
        Assert.Equal("Lemon Salmon", result.MenuItems[0].Name);

        Assert.Equal(result.Name, reloaded.Name);
        Assert.Equal(result.Type, reloaded.Type);
        Assert.Equal(result.Description, reloaded.Description);
        Assert.Equal(result.IsActive, reloaded.IsActive);

    }



        // ==================== Delete Menu Tests =============================

    //Delete Menu Test

    [Fact]
    public void Delete_Menu()
    {
        // arrange
        var ingredient = svc.AddIngredient("Potato");
        var menuItem = svc.AddMenuItem("Chips", "Crispy chips", 4.50, new List<Ingredient> { ingredient });
        var created = svc.AddMenu("Sides Menu", "Sides", "Side dishes", true, new List<MenuItem> { menuItem });

        // act
        var deleted = svc.DeleteMenu(created.Id);
        var found = svc.GetMenuById(created.Id);

        // assert
        Assert.True(deleted);
        Assert.Null(found);
        Assert.Empty(svc.SearchMenus());

    }


    // ==================== Ingredient Tests =============================

    [Fact]
    public void Can_add_ingredient()
    {
        var ingredient = svc.AddIngredient("Milk", true, "Contains dairy");

        Assert.NotNull(ingredient);
        Assert.True(ingredient.Id > 0);
        Assert.Equal("Milk", ingredient.Name);
        Assert.True(ingredient.Allergen);
        Assert.Equal("Contains dairy", ingredient.AllergenInfo);
    }

    [Fact]
    public void Can_get_all_ingredients()
    {
        svc.AddIngredient("Salt");
        svc.AddIngredient("Pepper");

        var ingredients = svc.GetAllIngredients();

        Assert.Equal(2, ingredients.Count);
        Assert.Contains(ingredients, i => i.Name == "Salt");
        Assert.Contains(ingredients, i => i.Name == "Pepper");
    }

    [Fact]
    public void Can_get_ingredient_by_id()
    {
        var ingredient = svc.AddIngredient("Butter");

        var found = svc.GetIngredientById(ingredient.Id);

        Assert.NotNull(found);
        Assert.Equal(ingredient.Id, found.Id);
        Assert.Equal("Butter", found.Name);
    }

    [Fact]
    public void Can_get_ingredient_by_name()
    {
        var ingredient = svc.AddIngredient("Garlic");

        var found = svc.GetIngredientByName("Garlic");

        Assert.NotNull(found);
        Assert.Equal(ingredient.Id, found.Id);
    }

    [Fact]
    public void Can_update_ingredient()
    {
        var ingredient = svc.AddIngredient("Nut", false, "No description");

        ingredient.Name = "Peanut";
        ingredient.Allergen = true;

        var updated = svc.UpdateIngredient(ingredient);

        Assert.NotNull(updated);
        Assert.Equal(ingredient.Id, updated.Id);
        Assert.Equal("Peanut", updated.Name);
        Assert.True(updated.Allergen);
    }

    [Fact]
    public void Can_delete_ingredient()
    {
        var ingredient = svc.AddIngredient("Onion");

        var deleted = svc.DeleteIngredient(ingredient.Id);
        var found = svc.GetIngredientById(ingredient.Id);

        Assert.True(deleted);
        Assert.Null(found);
    }


    // ==================== Menu Ordering Tests =============================

    [Fact]
    public void Can_get_all_menus_ordered_by_name_asc()
    {
        var ingredient = svc.AddIngredient("Base");
        var itemA = svc.AddMenuItem("Item A", "Desc", 1.0, new List<Ingredient> { ingredient });
        var itemB = svc.AddMenuItem("Item B", "Desc", 2.0, new List<Ingredient> { ingredient });

        svc.AddMenu("Z Menu", "Dinner", "Zed", true, new List<MenuItem> { itemA });
        svc.AddMenu("A Menu", "Lunch", "Alpha", true, new List<MenuItem> { itemB });

        var menus = svc.GetAllMenus("Name", "asc");

        Assert.Equal(2, menus.Count);
        Assert.Equal("A Menu", menus[0].Name);
        Assert.Equal("Z Menu", menus[1].Name);
    }

    [Fact]
    public void Can_get_all_menus_with_invalid_sort_defaults_to_id_asc()
    {
        var ingredient = svc.AddIngredient("Base");
        var item = svc.AddMenuItem("Item", "Desc", 1.0, new List<Ingredient> { ingredient });

        var menu1 = svc.AddMenu("Second", "Dinner", "D2", true, new List<MenuItem> { item });
        var menu2 = svc.AddMenu("First", "Lunch", "D1", true, new List<MenuItem> { item });

        var menus = svc.GetAllMenus("Invalid", "Invalid");

        Assert.Equal(2, menus.Count);
        Assert.Equal(menu1.Id, menus[0].Id);
        Assert.Equal(menu2.Id, menus[1].Id);
    }


    // ==================== MenuItem Tests =============================

    [Fact]
    public void Can_add_menu_item()
    {
        var ingredient1 = svc.AddIngredient("Flour");
        var ingredient2 = svc.AddIngredient("Eggs", true, "Contains eggs");

        var menuItem = svc.AddMenuItem("Pancakes", "Stack of pancakes", 7.50, new List<Ingredient> { ingredient1, ingredient2 });

        Assert.NotNull(menuItem);
        Assert.True(menuItem.Id > 0);
        Assert.Equal("Pancakes", menuItem.Name);
        Assert.Equal(2, menuItem.Ingredients.Count);
    }

    [Fact]
    public void Can_get_menu_item_by_id_with_ingredients()
    {
        var ingredient = svc.AddIngredient("Cheese", true, "Contains milk");
        var menuItem = svc.AddMenuItem("Cheese Toastie", "Toasted sandwich", 6.25, new List<Ingredient> { ingredient });

        var found = svc.GetMenuItemById(menuItem.Id);

        Assert.NotNull(found);
        Assert.Equal(menuItem.Id, found.Id);
        Assert.Single(found.Ingredients);
        Assert.Equal("Cheese", found.Ingredients[0].Name);
    }

    [Fact]
    public void Can_get_all_menu_items()
    {
        var ingredient = svc.AddIngredient("Tomato");
        svc.AddMenuItem("Soup", "Hot soup", 4.5, new List<Ingredient> { ingredient });
        svc.AddMenuItem("Bruschetta", "Starter", 5.0, new List<Ingredient> { ingredient });

        var items = svc.GetAllMenuItems();

        Assert.Equal(2, items.Count);
    }

    [Fact]
    public void Can_update_menu_item()
    {
        var ingredient1 = svc.AddIngredient("Rice");
        var ingredient2 = svc.AddIngredient("Chicken");

        var menuItem = svc.AddMenuItem("Rice Bowl", "Original", 9.0, new List<Ingredient> { ingredient1 });

        menuItem.Name = "Chicken Rice Bowl";
        menuItem.Description = "Updated";
        menuItem.Price = 10.5;
        menuItem.Ingredients = new List<Ingredient> { ingredient1, ingredient2 };

        var updated = svc.UpdateMenuItem(menuItem);

        Assert.NotNull(updated);
        Assert.Equal(menuItem.Id, updated.Id);
        Assert.Equal("Chicken Rice Bowl", updated.Name);
        Assert.Equal("Updated", updated.Description);
        Assert.Equal(10.5, updated.Price);
        Assert.Equal(2, updated.Ingredients.Count);
    }

    [Fact]
    public void Can_delete_menu_item()
    {
        var ingredient = svc.AddIngredient("Potato");
        var menuItem = svc.AddMenuItem("Mash", "Creamy mash", 3.5, new List<Ingredient> { ingredient });

        var deleted = svc.DeleteMenuItem(menuItem.Id);
        var found = svc.GetMenuItemById(menuItem.Id);

        Assert.True(deleted);
        Assert.Null(found);
    }

    [Fact]
    public void Can_get_ingredients_by_menu_item_id()
    {
        var ingredient1 = svc.AddIngredient("Beef");
        var ingredient2 = svc.AddIngredient("Soy", true, "Contains soy");

        var menuItem = svc.AddMenuItem("Beef Stir Fry", "With soy sauce", 12.0, new List<Ingredient> { ingredient1, ingredient2 });

        var ingredients = svc.GetIngredientsByMenuItemId(menuItem.Id);

        Assert.Equal(2, ingredients.Count);
        Assert.Contains(ingredients, i => i.Name == "Beef");
        Assert.Contains(ingredients, i => i.Name == "Soy");
    }








[Collection("Sequential")]
public class RestaurantServiceDbExtraTests
{
    private readonly RestaurantServiceDb svc;

    public RestaurantServiceDbExtraTests()
    {
        svc = new RestaurantServiceDb();
        svc.Initialise();
    }

    [Fact]
    public void Can_add_menu_items_to_existing_menu()
    {
        var ingredient = svc.AddIngredient("Chicken");
        var item1 = svc.AddMenuItem("Chicken Burger", "Burger", 10.0, new List<Ingredient> { ingredient });
        var item2 = svc.AddMenuItem("Chicken Wrap", "Wrap", 9.0, new List<Ingredient> { ingredient });

        var menu = svc.AddMenu("Chicken Menu", "Lunch", "Chicken dishes", true, new List<MenuItem> { item1 });

        var updated = svc.AddMenuItems(new List<MenuItem> { item2 }, menu.Id);

        Assert.NotNull(updated);
        Assert.Equal(menu.Id, updated.Id);
        Assert.Equal(2, updated.MenuItems.Count);
        Assert.Contains(updated.MenuItems, mi => mi.Name == "Chicken Burger");
        Assert.Contains(updated.MenuItems, mi => mi.Name == "Chicken Wrap");
    }

    [Fact]
    public void Add_menu_items_returns_null_when_menu_not_found()
    {
        var ingredient = svc.AddIngredient("Fish");
        var item = svc.AddMenuItem("Fish Taco", "Taco", 8.0, new List<Ingredient> { ingredient });

        var result = svc.AddMenuItems(new List<MenuItem> { item }, 9999);

        Assert.Null(result);
    }

    [Fact]
    public void Can_add_menu_item_to_order()
    {
        using var db = new DataContext();
        var order = new Order();
        db.Orders.Add(order);
        db.SaveChanges();

        var ingredient = svc.AddIngredient("Prawn");
        var menuItem = svc.AddMenuItem("Prawn Starter", "Starter", 6.0, new List<Ingredient> { ingredient });

        var updatedOrder = svc.AddMenuItemToOrder(order.Id, menuItem.Id);

        Assert.NotNull(updatedOrder);
        Assert.Equal(order.Id, updatedOrder.Id);
        Assert.Single(updatedOrder.MenuItems);
        Assert.Equal(menuItem.Id, updatedOrder.MenuItems[0].Id);
    }

    [Fact]
    public void Add_menu_item_to_order_returns_null_when_order_or_item_missing()
    {
        var ingredient = svc.AddIngredient("Beetroot");
        var menuItem = svc.AddMenuItem("Beet Salad", "Salad", 5.5, new List<Ingredient> { ingredient });

        var missingOrderResult = svc.AddMenuItemToOrder(9999, menuItem.Id);
        var missingItemResult = svc.AddMenuItemToOrder(1, 9999);

        Assert.Null(missingOrderResult);
        Assert.Null(missingItemResult);
    }
}


[Collection("Sequential")]
public class OrderServiceTests
{
    private readonly IRestaurantService svc;

    public OrderServiceTests()
    {
        svc = new RestaurantServiceDb();
        svc.Initialise();
    }

    [Fact]
    public void Can_add_order()
    {
        var ingredient = svc.AddIngredient("Chicken");
        var item1 = svc.AddMenuItem("Wrap", "Chicken wrap", 8.50, new List<Ingredient> { ingredient });
        var item2 = svc.AddMenuItem("Fries", "Crispy fries", 3.00, new List<Ingredient> { ingredient });

        var order = svc.AddOrder(new List<MenuItem> { item1, item2 });

        Assert.NotNull(order);
        Assert.True(order.Id > 0);
        Assert.Equal(2, order.MenuItems.Count);
        Assert.Equal(11.50, order.totalCost);
        Assert.Equal(11.50, order.FinalPrice);
    }

    [Fact]
    public void Can_get_all_orders()
    {
        var ingredient = svc.AddIngredient("Beef");
        var item = svc.AddMenuItem("Burger", "Beef burger", 9.00, new List<Ingredient> { ingredient });

        svc.AddOrder(new List<MenuItem> { item });
        svc.AddOrder(new List<MenuItem> { item });

        var orders = svc.GetAllOrders();

        Assert.Equal(2, orders.Count);
    }

    [Fact]
    public void Can_get_order_by_id()
    {
        var ingredient = svc.AddIngredient("Salmon");
        var item = svc.AddMenuItem("Salmon Plate", "Grilled salmon", 14.00, new List<Ingredient> { ingredient });
        var order = svc.AddOrder(new List<MenuItem> { item });

        var found = svc.GetOrderById(order.Id);

        Assert.NotNull(found);
        Assert.Equal(order.Id, found.Id);
        Assert.Single(found.MenuItems);
        Assert.Equal("Salmon Plate", found.MenuItems[0].Name);
    }

    [Fact]
    public void Can_get_orders_by_table_id()
    {
        int tableId;
        using (var db = new DataContext())
        {
            var table = new Table
            {
                TableNumber = 12,
                SeatingCapacity = 4,
                CustomersSeated = 2,
                IsOccupied = true,
                OrderNumber = 1,
                Active = true
            };
            db.Tables.Add(table);
            db.SaveChanges();
            tableId = table.Id;
        }

        var ingredient = svc.AddIngredient("Pasta");
        var item = svc.AddMenuItem("Pasta", "Creamy pasta", 10.00, new List<Ingredient> { ingredient });
        svc.AddOrder(new List<MenuItem> { item }, tableId);

        var orders = svc.GetOrdersByTableId(tableId);

        Assert.Single(orders);
        Assert.NotNull(orders[0].Table);
        Assert.Equal(tableId, orders[0].Table.Id);
    }

    [Fact]
    public void Can_update_order()
    {
        var ingredient = svc.AddIngredient("Rice");
        var item1 = svc.AddMenuItem("Rice Bowl", "Original", 9.00, new List<Ingredient> { ingredient });
        var item2 = svc.AddMenuItem("Rice Deluxe", "Updated", 12.00, new List<Ingredient> { ingredient });
        var order = svc.AddOrder(new List<MenuItem> { item1 });

        var updatePayload = new Order
        {
            Id = order.Id,
            MenuItems = new List<MenuItem> { item2 },
            discount = 10,
            IsCompleted = true,
            IsVoid = false,
            AllergyCountRequired = 1
        };

        var updated = svc.UpdateOrder(updatePayload);

        Assert.NotNull(updated);
        Assert.Equal(order.Id, updated.Id);
        Assert.Single(updated.MenuItems);
        Assert.Equal("Rice Deluxe", updated.MenuItems[0].Name);
        Assert.Equal(12.00, updated.totalCost);
        Assert.Equal(10, updated.discount);
        Assert.True(updated.IsCompleted);
        Assert.False(updated.IsVoid);
        Assert.Equal(1, updated.AllergyCountRequired);
        Assert.Equal(10.80, updated.FinalPrice);
    }

    [Fact]
    public void Can_delete_order()
    {
        var ingredient = svc.AddIngredient("Potato");
        var item = svc.AddMenuItem("Wedges", "Potato wedges", 4.00, new List<Ingredient> { ingredient });
        var order = svc.AddOrder(new List<MenuItem> { item });

        var deleted = svc.DeleteOrder(order.Id);
        var found = svc.GetOrderById(order.Id);

        Assert.True(deleted);
        Assert.Null(found);
    }

    [Fact]
    public void Can_mark_order_completed()
    {
        var ingredient = svc.AddIngredient("Chicken");
        var item = svc.AddMenuItem("Chicken Bites", "Starter", 5.00, new List<Ingredient> { ingredient });
        var order = svc.AddOrder(new List<MenuItem> { item });

        var updated = svc.MarkOrderCompleted(order.Id, true);

        Assert.NotNull(updated);
        Assert.True(updated.IsCompleted);
    }

    [Fact]
    public void Can_void_order()
    {
        var ingredient = svc.AddIngredient("Bread");
        var item = svc.AddMenuItem("Toast", "Buttered toast", 2.00, new List<Ingredient> { ingredient });
        var order = svc.AddOrder(new List<MenuItem> { item });

        var updated = svc.VoidOrder(order.Id, true);

        Assert.NotNull(updated);
        Assert.True(updated.IsVoid);
    }
}


[Collection("Sequential")]
public class TableServiceTests
{
    private readonly IRestaurantService svc;

    public TableServiceTests()
    {
        svc = new RestaurantServiceDb();
        svc.Initialise();
    }

    [Fact]
    public void Can_add_table()
    {
        var table = svc.AddTable(10, 4, 0, false, true);

        Assert.NotNull(table);
        Assert.True(table.Id > 0);
        Assert.Equal(10, table.TableNumber);
        Assert.Equal(4, table.SeatingCapacity);
        Assert.False(table.IsOccupied);
        Assert.True(table.Active);
    }

    [Fact]
    public void Can_get_all_tables()
    {
        svc.AddTable(1, 2);
        svc.AddTable(2, 4);

        var tables = svc.GetAllTables();

        Assert.Equal(2, tables.Count);
    }

    [Fact]
    public void Can_get_table_by_id()
    {
        var table = svc.AddTable(6, 4);

        var found = svc.GetTableById(table.Id);

        Assert.NotNull(found);
        Assert.Equal(table.Id, found.Id);
        Assert.Equal(6, found.TableNumber);
    }

    [Fact]
    public void Can_get_table_by_table_number()
    {
        svc.AddTable(3, 2);
        var table = svc.AddTable(8, 6);

        var found = svc.GetTableByTableNumber(8);

        Assert.NotNull(found);
        Assert.Equal(table.Id, found.Id);
    }

    [Fact]
    public void Can_update_table()
    {
        var table = svc.AddTable(4, 2, 0, false, true);

        table.TableNumber = 14;
        table.SeatingCapacity = 6;
        table.IsOccupied = true;
        table.CustomersSeated = 4;
        table.OrderNumber = 2;
        table.Active = true;

        var updated = svc.UpdateTable(table);

        Assert.NotNull(updated);
        Assert.Equal(14, updated.TableNumber);
        Assert.Equal(6, updated.SeatingCapacity);
        Assert.True(updated.IsOccupied);
        Assert.Equal(4, updated.CustomersSeated);
        Assert.Equal(2, updated.OrderNumber);
    }

    [Fact]
    public void Can_delete_table()
    {
        var table = svc.AddTable(7, 4);

        var deleted = svc.DeleteTable(table.Id);
        var found = svc.GetTableById(table.Id);

        Assert.True(deleted);
        Assert.Null(found);
    }

    [Fact]
    public void Can_set_table_occupancy()
    {
        var table = svc.AddTable(9, 4, 0, false, true);

        var occupied = svc.SetTableOccupancy(table.Id, true, 3);

        Assert.NotNull(occupied);
        Assert.True(occupied.IsOccupied);
        Assert.Equal(3, occupied.CustomersSeated);

        var cleared = svc.SetTableOccupancy(table.Id, false, 0);

        Assert.NotNull(cleared);
        Assert.False(cleared.IsOccupied);
        Assert.Equal(0, cleared.CustomersSeated);
    }

    [Fact]
    public void Can_get_available_tables()
    {
        svc.AddTable(1, 2, 0, false, true);
        svc.AddTable(2, 2, 2, true, true);
        svc.AddTable(3, 6, 0, false, true);

        var available = svc.GetAvailableTables(4);

        Assert.Single(available);
        Assert.Equal(3, available[0].TableNumber);
    }
}


[Collection("Sequential")]
public class BookingServiceTests
{
    private readonly IRestaurantService svc;

    public BookingServiceTests()
    {
        svc = new RestaurantServiceDb();
        svc.Initialise();
    }

    [Fact]
    public void Can_add_booking()
    {
        var bookingDate = new DateTime(2026, 3, 10, 19, 0, 0);

        var booking = svc.AddBooking(
            "Corey Gallagher",
            "07123456789",
            "corey@example.com",
            bookingDate,
            4,
            true,
            true,
            0,
            "Window table",
            12,
            true
        );

        Assert.NotNull(booking);
        Assert.True(booking.Id > 0);
        Assert.Equal("Corey Gallagher", booking.CustomerName);
        Assert.Equal(4, booking.NumberOfGuests);
        Assert.True(booking.HasAllergen);
        Assert.True(booking.AllergenConsentGiven);
        Assert.Equal(12, booking.TableNumber);
    }

    [Fact]
    public void Can_get_all_bookings()
    {
        svc.AddBooking("A", "1", "a@test.com", DateTime.UtcNow.AddHours(1), 2);
        svc.AddBooking("B", "2", "b@test.com", DateTime.UtcNow.AddHours(2), 4);

        var bookings = svc.GetAllBookings();

        Assert.Equal(2, bookings.Count);
    }

    [Fact]
    public void Can_get_booking_by_id()
    {
        var booking = svc.AddBooking("John", "1", "j@test.com", DateTime.UtcNow.AddHours(1), 2);

        var found = svc.GetBookingById(booking.Id);

        Assert.NotNull(found);
        Assert.Equal(booking.Id, found.Id);
        Assert.Equal("John", found.CustomerName);
    }

    [Fact]
    public void Can_get_bookings_by_date()
    {
        var targetDate = new DateTime(2026, 3, 20, 18, 30, 0);

        svc.AddBooking("A", "1", "a@test.com", targetDate, 2);
        svc.AddBooking("B", "2", "b@test.com", targetDate.AddHours(1), 3);
        svc.AddBooking("C", "3", "c@test.com", targetDate.AddDays(1), 4);

        var bookings = svc.GetBookingsByDate(targetDate);

        Assert.Equal(2, bookings.Count);
    }

    [Fact]
    public void Can_get_active_bookings()
    {
        svc.AddBooking("A", "1", "a@test.com", DateTime.UtcNow.AddHours(1), 2, isActive: true);
        svc.AddBooking("B", "2", "b@test.com", DateTime.UtcNow.AddHours(2), 4, isActive: false);

        var activeBookings = svc.GetActiveBookings();

        Assert.Single(activeBookings);
        Assert.True(activeBookings[0].IsActive);
    }

    [Fact]
    public void Can_update_booking()
    {
        var booking = svc.AddBooking("Jane", "1", "j@test.com", DateTime.UtcNow.AddHours(1), 2);

        booking.CustomerName = "Jane Updated";
        booking.PhoneNumber = "07000000000";
        booking.Email = "updated@test.com";
        booking.NumberOfGuests = 5;
        booking.HasAllergen = true;
        booking.AllergenConsentGiven = true;
        booking.BookingComments = "Birthday";
        booking.TableNumber = 15;
        booking.IsActive = false;

        var updated = svc.UpdateBooking(booking);

        Assert.NotNull(updated);
        Assert.Equal("Jane Updated", updated.CustomerName);
        Assert.Equal("07000000000", updated.PhoneNumber);
        Assert.Equal("updated@test.com", updated.Email);
        Assert.Equal(5, updated.NumberOfGuests);
        Assert.True(updated.HasAllergen);
        Assert.True(updated.AllergenConsentGiven);
        Assert.Equal("Birthday", updated.BookingComments);
        Assert.Equal(15, updated.TableNumber);
        Assert.False(updated.IsActive);
    }

    [Fact]
    public void Can_delete_booking()
    {
        var booking = svc.AddBooking("Delete Me", "1", "d@test.com", DateTime.UtcNow.AddHours(1), 2);

        var deleted = svc.DeleteBooking(booking.Id);
        var found = svc.GetBookingById(booking.Id);

        Assert.True(deleted);
        Assert.Null(found);
    }

    [Fact]
    public void Can_set_booking_active_status()
    {
        var booking = svc.AddBooking("Toggle", "1", "t@test.com", DateTime.UtcNow.AddHours(1), 2, isActive: true);

        var updated = svc.SetBookingActiveStatus(booking.Id, false);

        Assert.NotNull(updated);
        Assert.False(updated.IsActive);
    }
}

}







