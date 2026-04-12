using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using RMS.Data.Entities;
using RMS.Data.Repository;

namespace RMS.Data.Services;

// EntityFramework Implementation of IRestaurantService
public class RestaurantServiceDb : IRestaurantService
{
    private readonly DataContext db;

    public RestaurantServiceDb()
    {
        db = new DataContext();
    }

    public void Initialise()
    {
        db.Initialise(); // recreate database
    }



    // -------- AllergenConsent Related Operations ------------




    // Add Allergen Consent
    public AllergenConsent AddAllergenConsent(int orderId, string customerName, string customerEmail, string customerPhone, bool consentGiven, List<MenuItem> menuItems)
    {
        var order = db.Orders.FirstOrDefault(o => o.Id == orderId);

        if (order == null)
        {
            return null; // OrderItem not found
        }

        var consent = new AllergenConsent
        {
            OrderId = orderId,
            CustomerName = customerName,
            CustomerEmail = customerEmail,
            CustomerPhone = customerPhone,
            ConsentGiven = consentGiven,
            MenuItems = menuItems
        };

        db.AllergenConsents.Add(consent);
        db.SaveChanges();
        return consent;
    }


    // Get All Allergen Consents
    public List<AllergenConsent> GetAllergenConsents()
    {
        return db.AllergenConsents.ToList();
    }

    // Get Allergen Consent by ID
    public bool OrderHasAllergenConsent(int orderId)
    {
        return db.AllergenConsents.Any(ac => ac.OrderId == orderId && ac.ConsentGiven);
    }   

    // Get Update Allergen Consent
    public void UpdateAllergenConsent(int consentId, string customerName, string customerEmail, string customerPhone, bool consentGiven)
    {
        var consent = db.AllergenConsents.FirstOrDefault(ac => ac.Id == consentId);
        if (consent != null)
        {
            consent.CustomerName = customerName;
            consent.CustomerEmail = customerEmail;
            consent.CustomerPhone = customerPhone;
            consent.ConsentGiven = consentGiven;

            db.AllergenConsents.Update(consent);
            db.SaveChanges();
        }
    }

    // Delete Allergen Consent
    public void DeleteAllergenConsent(int consentId)
    {
        var consent = db.AllergenConsents
            .Include(ac => ac.MenuItems)
            .FirstOrDefault(ac => ac.Id == consentId);
        if (consent != null)
        {
            consent.MenuItems.Clear(); // remove join entries before deleting parent
            db.AllergenConsents.Remove(consent);
            db.SaveChanges();
        }
    }   


    // Get Allergen Consents by Order ID
    public List<AllergenConsent> GetAllergenConsentsByOrderId(int orderId)
    {
        return db.AllergenConsents.Where(ac => ac.OrderId == orderId).ToList();
    }


    // Get Consent Given Count by Order ID
    public int GetConsentGivenCountByOrderId(int orderId)
    {
        return db.AllergenConsents.Count(ac => ac.OrderId == orderId && ac.ConsentGiven);
    }





    // -------- Ingredient Related Operations ------------
    

    // Add Ingredient
    public Ingredient AddIngredient(string name, bool allergen = false, string description = "No description")
    {
        var ingredient = new Ingredient
        {
            Name = name,
            Allergen = allergen,
            AllergenInfo = description
        };

        db.Ingredients.Add(ingredient);
        db.SaveChanges();
        return ingredient;
    }

    // Get all Ingredients
    public List<Ingredient> GetAllIngredients()
    {
        return db.Ingredients.ToList();
    }

    // Get Ingredient by ID
    public Ingredient GetIngredientById(int id)
    {
        return db.Ingredients.FirstOrDefault(i => i.Id == id);
    }

    // Get Ingredient by Name
    public Ingredient GetIngredientByName(string name)
    {
        return db.Ingredients.FirstOrDefault(i => i.Name == name);
    }

    // Delete Ingredient
    public bool DeleteIngredient(int id)
    {
        var ingredient = GetIngredientById(id);
        if (ingredient == null)
        {
            return false; // Ingredient not found
        }

        db.Ingredients.Remove(ingredient);
        db.SaveChanges();
        return true;
    }
  
    // Update Ingredient
    public Ingredient UpdateIngredient(Ingredient ingredient)
    {
        var existingIngredient = GetIngredientById(ingredient.Id);
        if (existingIngredient == null)
        {
            return null; // Ingredient not found
        }

        existingIngredient.Name = ingredient.Name;
        existingIngredient.Allergen = ingredient.Allergen;

        db.Ingredients.Update(existingIngredient);
        db.SaveChanges();
        return existingIngredient;
    }

    // Get Ingredients by MenuItem ID

    public List<Ingredient> GetIngredientsByMenuItemId(int menuItemId)
    {
        return db.MenuItems
            .Where(mi => mi.Id == menuItemId)
            .SelectMany(mi => mi.Ingredients)
            .ToList();
    }





    // -------- Menu Related Operations ------------
    

    // Get all Menus

    public List<Menu> GetAllMenus(string orderBy="Id", string direction="asc")
    {
        return (orderBy,direction) switch 
        {
            ("Id","asc") => db.Menus.OrderBy(m => m.Id).ToList(),
            ("Id","desc") => db.Menus.OrderByDescending(m => m.Id).ToList(),

            ("Name","asc") => db.Menus.OrderBy(m => m.Name).ToList(),
            ("Name","desc") => db.Menus.OrderByDescending(m => m.Name).ToList(),
            
            ("Type","asc") => db.Menus.OrderBy(m => m.Type).ToList(),
            ("Type","desc") => db.Menus.OrderByDescending(m => m.Type).ToList(),
            
            ("Description","asc") => db.Menus.OrderBy(m => m.Description).ToList(),
            ("Description","desc") => db.Menus.OrderByDescending(m => m.Description).ToList(),
            
            ("IsActive","asc") => db.Menus.OrderBy(m => m.IsActive).ToList(),

            ("IsActive","desc") => db.Menus.OrderByDescending(m => m.IsActive).ToList(),

            ("CreatedOn","asc") => db.Menus.OrderBy(m => m.CreatedOn).ToList(),
            ("CreatedOn","desc") => db.Menus.OrderByDescending(m => m.CreatedOn).ToList(),
            
            _ => db.Menus.OrderBy(m => m.Id).ToList()          
        };

      
    }


    // Get Menu by ID
    public Menu GetMenuById(int id) 
    {
        return db.Menus
            .Include(m => m.MenuItems)
            .FirstOrDefault(m => m.Id == id);

    }

    // perform a search of the Menus
    public IList<Menu>SearchMenus() 
    {
        return db.Menus
            .Include(m => m.MenuItems)
            .OrderBy(m => m.Id)
            .ToList();
    }

                    
    //Get Menu By Name

    public Menu GetMenuByName(string name)
    {
        return db.Menus.FirstOrDefault(m => m.Name == name);
    }


    // Add Menu
    public Menu AddMenu(string menuName, string menuType, string menuDescription, bool isActive, List<MenuItem> menuItems)
    {
     
        // add Menu
        Menu m = new Menu{
            Name = menuName,
            Type = menuType,
            Description = menuDescription,
            IsActive = isActive,
            MenuItems = menuItems
        };

        db.Menus.Add(m);
        db.SaveChanges();
        return m;
    }


    // Delete Menu 
    public bool DeleteMenu (int id)
    {
        var m = GetMenuById(id);

        // Check if Menu is found
        if(m == null){return false;}

        // remove the Menu
        db.Menus.Remove(m);
        db.SaveChanges();
        return true;

    }


    // Update Menu 
    public Menu UpdateMenu (Menu m)
    {

        // Check if Menu exists
        var originalMenu = GetMenuById(m.Id);   
        if(originalMenu == null){return null;}
       
        // update the Menu (MenuItems managed separately via AddMenuItemToMenu/RemoveMenuItemFromMenu)
        originalMenu.Name = m.Name;
        originalMenu.Type = m.Type;
        originalMenu.Description = m.Description;
        originalMenu.IsActive = m.IsActive;

        db.Menus.Update(originalMenu);
        db.SaveChanges();
        
        return GetMenuById(originalMenu.Id);

    }
 // -------- MenuItem Related Operations ------------

   // Add MenuItem
    public MenuItem AddMenuItem(string name, string type, string description, double price, List<Ingredient> ingredients)
    {
        var menuItem = new MenuItem
        {
            Name = name,
            Type = type,
            Description = description,
            Price = price,
            Ingredients = ingredients
        };

        db.MenuItems.Add(menuItem);
        db.SaveChanges();
        return menuItem;
    }

    // Get MenuItem by ID
    public MenuItem GetMenuItemById(int id)
    {
        return db.MenuItems
            .Include(mi => mi.Ingredients)
            .FirstOrDefault(mi => mi.Id == id);
    }

     // Get all MenuItems
    public List<MenuItem> GetAllMenuItems()
    {
        return db.MenuItems
            .Include(mi => mi.Ingredients)
            .ToList();
    }

     // Delete MenuItem 
    public bool DeleteMenuItem (int id)
    {
        var mi = GetMenuItemById(id);

        // Check if MenuItem is found
        if(mi == null){return false;}

        // remove the MenuItem
        db.MenuItems.Remove(mi);
        db.SaveChanges();
        return true;

    }

    // Update MenuItem 

    public MenuItem UpdateMenuItem (MenuItem mi)


    {

        // Check if MenuItem exists
        var originalMenuItem = GetMenuItemById(mi.Id);   
        if(originalMenuItem == null){return null;}
       
        // update the MenuItem
        originalMenuItem.Name = mi.Name;
        originalMenuItem.Description = mi.Description;
        originalMenuItem.Price = mi.Price;
        originalMenuItem.Ingredients = mi.Ingredients;
        originalMenuItem.Type = mi.Type;

        db.MenuItems.Update(originalMenuItem);
        db.SaveChanges();
        
        return GetMenuItemById(originalMenuItem.Id);

    }


    // Add MenuItems to Menu
    public Menu AddMenuItems(List<MenuItem> menuItems, int menuId)
    {
        var menu = GetMenuById(menuId);
        if (menu == null)
        {
            return null; // Menu not found
        }

        menu.MenuItems.AddRange(menuItems);
        db.Menus.Update(menu);
        db.SaveChanges();
        return GetMenuById(menuId);
    }

    public Menu AddMenuItemToMenu(int menuId, int menuItemId)
    {
        var menu = GetMenuById(menuId);
        var item = GetMenuItemById(menuItemId);
        if (menu == null || item == null) return null;
        if (menu.MenuItems.Any(i => i.Id == menuItemId)) return menu;
        menu.MenuItems.Add(item);
        db.SaveChanges();
        return GetMenuById(menuId);
    }

    public Menu RemoveMenuItemFromMenu(int menuId, int menuItemId)
    {
        var menu = GetMenuById(menuId);
        if (menu == null) return null;
        var item = menu.MenuItems.FirstOrDefault(i => i.Id == menuItemId);
        if (item != null)
        {
            menu.MenuItems.Remove(item);
            db.SaveChanges();
        }
        return GetMenuById(menuId);
    }
    

   // Add MenuItem to Order
    public Order AddMenuItemToOrder(int orderId, int menuItemId)
    {
        var order = db.Orders.FirstOrDefault(o => o.Id == orderId);
        var menuItem = GetMenuItemById(menuItemId);

        if (order == null || menuItem == null)
        {
            return null; // Order or MenuItem not found
        }

        order.MenuItems.Add(menuItem);
        db.Orders.Update(order);
        db.SaveChanges();
        return order;
    }


 // -------- Order Related Operations ------------

    // Add Order
    public Order AddOrder(List<MenuItem> menuItems, int? tableId = null)
    {
        Table table = null;
        if (tableId.HasValue)
        {
            table = db.Tables.FirstOrDefault(t => t.Id == tableId.Value);
        }

        var items = menuItems ?? new List<MenuItem>();
        var totalCost = items.Sum(mi => mi.Price);

        var order = new Order
        {
            MenuItems = items,
            Table = table,
            totalCost = totalCost,
            FinalPrice = totalCost
        };

        db.Orders.Add(order);

        // Mark the table as occupied when an order is placed on it
        if (table != null)
        {
            table.IsOccupied = true;
            db.Tables.Update(table);
        }

        db.SaveChanges();
        return GetOrderById(order.Id);
    }

    // Get all Orders
    public List<Order> GetAllOrders()
    {
        return db.Orders
            .Include(o => o.MenuItems)
            .Include(o => o.Table)
            .ToList();
    }

    // Get Order by ID
    public Order GetOrderById(int id)
    {
        return db.Orders
            .Include(o => o.MenuItems)
            .Include(o => o.Table)
            .FirstOrDefault(o => o.Id == id);
    }

    // Get Orders by Table ID
    public List<Order> GetOrdersByTableId(int tableId)
    {
        return db.Orders
            .Include(o => o.MenuItems)
            .Include(o => o.Table)
            .Where(o => o.Table != null && o.Table.Id == tableId)
            .ToList();
    }

    // Update Order
    public Order UpdateOrder(Order order)
    {
        var existingOrder = GetOrderById(order.Id);
        if (existingOrder == null)
        {
            return null; // Order not found
        }

        existingOrder.MenuItems = order.MenuItems ?? new List<MenuItem>();
        existingOrder.totalCost = existingOrder.MenuItems.Sum(mi => mi.Price);
        existingOrder.discount = order.discount;
        existingOrder.IsCompleted = order.IsCompleted;
        existingOrder.IsVoid = order.IsVoid;
        existingOrder.AllergyCountRequired = order.AllergyCountRequired;
        existingOrder.FinalPrice = existingOrder.totalCost - (existingOrder.totalCost * existingOrder.discount / 100.0);

        if (order.Table != null)
        {
            var table = db.Tables.FirstOrDefault(t => t.Id == order.Table.Id);
            existingOrder.Table = table;
        }

        db.Orders.Update(existingOrder);
        db.SaveChanges();
        return GetOrderById(existingOrder.Id);
    }

    // Delete Order
    public bool DeleteOrder(int id)
    {
        var order = GetOrderById(id);
        if (order == null)
        {
            return false; // Order not found
        }

        db.Orders.Remove(order);
        db.SaveChanges();
        return true;
    }

    // Mark Order Completed
    public Order MarkOrderCompleted(int id, bool isCompleted = true)
    {
        var order = GetOrderById(id);
        if (order == null)
        {
            return null; // Order not found
        }

        order.IsCompleted = isCompleted;
        db.Orders.Update(order);

        // Free the table when the order is completed
        if (order.Table != null)
        {
            order.Table.IsOccupied = false;
            db.Tables.Update(order.Table);
        }

        db.SaveChanges();
        return GetOrderById(order.Id);
    }

    // Void Order
    public Order VoidOrder(int id, bool isVoid = true)
    {
        var order = GetOrderById(id);
        if (order == null)
        {
            return null; // Order not found
        }

        order.IsVoid = isVoid;
        db.Orders.Update(order);

        // Free the table when the order is voided
        if (order.Table != null)
        {
            order.Table.IsOccupied = false;
            db.Tables.Update(order.Table);
        }

        db.SaveChanges();
        return GetOrderById(order.Id);
    }

    public Order UpdateCourseStatus(int id, string courseStatus)
    {
        var order = GetOrderById(id);
        if (order == null) { return null; }
        order.CourseStatus = courseStatus;
        db.Orders.Update(order);
        db.SaveChanges();
        return GetOrderById(id);
    }


 // -------- Table Related Operations ------------

    // Add Table
    public Table AddTable(int tableNumber, int seatingCapacity, int customersSeated = 0, bool isOccupied = false, bool active = true)
    {
        var table = new Table
        {
            TableNumber = tableNumber,
            SeatingCapacity = seatingCapacity,
            CustomersSeated = customersSeated,
            IsOccupied = isOccupied,
            Active = active,
            OrderNumber = 0
        };

        db.Tables.Add(table);
        db.SaveChanges();
        return table;
    }

    // Get all Tables
    public List<Table> GetAllTables()
    {
        return db.Tables
            .Include(t => t.Orders)
            .OrderBy(t => t.Id)
            .ToList();
    }

    // Get Table by ID
    public Table GetTableById(int id)
    {
        return db.Tables
            .Include(t => t.Orders)
            .FirstOrDefault(t => t.Id == id);
    }

    // Get Table by Table Number
    public Table GetTableByTableNumber(int tableNumber)
    {
        return db.Tables
            .Include(t => t.Orders)
            .FirstOrDefault(t => t.TableNumber == tableNumber);
    }

    // Update Table
    public Table UpdateTable(Table table)
    {
        var existingTable = GetTableById(table.Id);
        if (existingTable == null)
        {
            return null; // Table not found
        }

        existingTable.TableNumber = table.TableNumber;
        existingTable.SeatingCapacity = table.SeatingCapacity;
        existingTable.CustomersSeated = table.CustomersSeated;
        existingTable.IsOccupied = table.IsOccupied;
        existingTable.OrderNumber = table.OrderNumber;
        existingTable.Active = table.Active;

        db.Tables.Update(existingTable);
        db.SaveChanges();
        return GetTableById(existingTable.Id);
    }

    // Delete Table
    public bool DeleteTable(int id)
    {
        var table = GetTableById(id);
        if (table == null)
        {
            return false; // Table not found
        }

        db.Tables.Remove(table);
        db.SaveChanges();
        return true;
    }

    // Set Table Occupancy
    public Table SetTableOccupancy(int id, bool isOccupied, int customersSeated = 0)
    {
        var table = GetTableById(id);
        if (table == null)
        {
            return null; // Table not found
        }

        table.IsOccupied = isOccupied;
        table.CustomersSeated = isOccupied ? customersSeated : 0;

        db.Tables.Update(table);
        db.SaveChanges();
        return GetTableById(table.Id);
    }

    // Get Available Tables
    public List<Table> GetAvailableTables(int minimumCapacity = 1)
    {
        return db.Tables
            .Where(t => t.Active && !t.IsOccupied && t.SeatingCapacity >= minimumCapacity)
            .OrderBy(t => t.TableNumber)
            .ToList();
    }


 // -------- Booking Related Operations ------------

    // Add Booking
    public Booking AddBooking(string customerName, string phoneNumber, string email, DateTime bookingDateTime, int numberOfGuests, bool hasAllergen = false, bool allergenConsentGiven = false, int orderId = 0, string bookingComments = "", int tableNumber = 0, bool isActive = true)
    {
        var booking = new Booking
        {
            CustomerName = customerName,
            PhoneNumber = phoneNumber,
            Email = email,
            BookingDateTime = bookingDateTime,
            NumberOfGuests = numberOfGuests,
            HasAllergen = hasAllergen,
            AllergenConsentGiven = allergenConsentGiven,
            OrderId = orderId,
            BookingComments = bookingComments,
            TableNumber = tableNumber,
            IsActive = isActive,
            Status = "Booked"
        };

        db.Bookings.Add(booking);
        db.SaveChanges();
        return booking;
    }

    // Get all Bookings
    public List<Booking> GetAllBookings()
    {
        return db.Bookings
            .OrderBy(b => b.Id)
            .ToList();
    }

    // Get Booking by ID
    public Booking GetBookingById(int id)
    {
        return db.Bookings.FirstOrDefault(b => b.Id == id);
    }

    // Get Bookings by Date
    public List<Booking> GetBookingsByDate(DateTime date)
    {
        return db.Bookings
            .Where(b => b.BookingDateTime.Date == date.Date)
            .OrderBy(b => b.BookingDateTime)
            .ToList();
    }

    // Get Active Bookings
    public List<Booking> GetActiveBookings()
    {
        return db.Bookings
            .Where(b => b.IsActive)
            .OrderBy(b => b.BookingDateTime)
            .ToList();
    }

    // Update Booking
    public Booking UpdateBooking(Booking booking)
    {
        var existingBooking = GetBookingById(booking.Id);
        if (existingBooking == null)
        {
            return null; // Booking not found
        }

        existingBooking.CustomerName = booking.CustomerName;
        existingBooking.PhoneNumber = booking.PhoneNumber;
        existingBooking.Email = booking.Email;
        existingBooking.BookingDateTime = booking.BookingDateTime;
        existingBooking.NumberOfGuests = booking.NumberOfGuests;
        existingBooking.HasAllergen = booking.HasAllergen;
        existingBooking.AllergenConsentGiven = booking.AllergenConsentGiven;
        existingBooking.OrderId = booking.OrderId;
        existingBooking.BookingComments = booking.BookingComments;
        existingBooking.TableNumber = booking.TableNumber;
        existingBooking.IsActive = booking.IsActive;
        existingBooking.Status = booking.Status;

        db.Bookings.Update(existingBooking);
        db.SaveChanges();
        return GetBookingById(existingBooking.Id);
    }

    // Delete Booking
    public bool DeleteBooking(int id)
    {
        var booking = GetBookingById(id);
        if (booking == null)
        {
            return false; // Booking not found
        }

        db.Bookings.Remove(booking);
        db.SaveChanges();
        return true;
    }

    // Set Booking Active Status
    public Booking SetBookingActiveStatus(int id, bool isActive)
    {
        var booking = GetBookingById(id);
        if (booking == null)
        {
            return null; // Booking not found
        }

        booking.IsActive = isActive;
        db.Bookings.Update(booking);
        db.SaveChanges();
        return GetBookingById(booking.Id);
    }


    // -------- Review Related Operations ------------

    public Review AddReview(int orderId, string customerName, int stars, string comment)
    {
        var order = db.Orders.FirstOrDefault(o => o.Id == orderId);
        if (order == null || !order.IsCompleted)
        {
            return null; // Can only review a completed order
        }

        // One review per order
        bool alreadyReviewed = db.Reviews.Any(r => r.OrderId == orderId);
        if (alreadyReviewed)
        {
            return null;
        }

        var review = new Review
        {
            OrderId = orderId,
            CustomerName = customerName,
            Stars = stars,
            Comment = comment
        };

        db.Reviews.Add(review);
        db.SaveChanges();
        return review;
    }

    public List<Review> GetAllReviews()
    {
        return db.Reviews.ToList();
    }

    public Review GetReviewById(int id)
    {
        return db.Reviews.FirstOrDefault(r => r.Id == id);
    }

    public Review GetReviewByOrderId(int orderId)
    {
        return db.Reviews.FirstOrDefault(r => r.OrderId == orderId);
    }

    public bool DeleteReview(int id)
    {
        var review = db.Reviews.FirstOrDefault(r => r.Id == id);
        if (review == null) { return false; }
        db.Reviews.Remove(review);
        db.SaveChanges();
        return true;
    }


}