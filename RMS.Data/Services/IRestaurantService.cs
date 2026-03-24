using System;
using System.Collections.Generic;
	
using RMS.Data.Entities;

//class SMS.Data.Entities.Ticket;

namespace RMS.Data.Services;

// This interface describes the operations that a MenuService class should implement
public interface IRestaurantService
{
    void Initialise();
        
    // add suitable method definitions to implement assignment requirements     


    // ------------- Allergen Consent Management -------------------

    AllergenConsent AddAllergenConsent(int orderId, string customerName, string customerEmail, string customerPhone, bool consentGiven, List<MenuItem> menuItems);


    List<AllergenConsent> GetAllergenConsents();

    bool OrderHasAllergenConsent(int orderId);


    void UpdateAllergenConsent(int consentId, string customerName, string customerEmail, string customerPhone, bool consentGiven);

    void DeleteAllergenConsent(int consentId);

    List<AllergenConsent> GetAllergenConsentsByOrderId(int orderId);

    int GetConsentGivenCountByOrderId(int orderId);




    // ------------- Ingredient Management -------------------

    Ingredient AddIngredient(string name, bool allergen = false, string allergenInfo = "No known allergen");

    List<Ingredient> GetAllIngredients();

    Ingredient GetIngredientById(int id);

    Ingredient UpdateIngredient(Ingredient ingredient);

    bool DeleteIngredient(int id);

    Ingredient GetIngredientByName(string name);


    public List<Ingredient> GetIngredientsByMenuItemId(int menuItemId);



     // ---------------- Menu Management ---------------

    
    List<Menu> GetAllMenus(string orderBy="Id", string direction="asc");

   
    Menu AddMenu(string menuName, string menuType, string menuDescription, bool isActive, List<MenuItem> menuItems);

         
    Menu GetMenuById(int id);

    Menu GetMenuByName(string name);    

    bool  DeleteMenu(int id);
    
    Menu UpdateMenu (Menu m);

    IList<Menu> SearchMenus();


    // ---------------- Order Management ---------------

    Order AddOrder(List<MenuItem> menuItems, int? tableId = null);

    List<Order> GetAllOrders();

    Order GetOrderById(int id);

    List<Order> GetOrdersByTableId(int tableId);

    Order UpdateOrder(Order order);

    bool DeleteOrder(int id);

    Order MarkOrderCompleted(int id, bool isCompleted = true);

    Order VoidOrder(int id, bool isVoid = true);


    // ---------------- Table Management ---------------

    Table AddTable(int tableNumber, int seatingCapacity, int customersSeated = 0, bool isOccupied = false, bool active = true);

    List<Table> GetAllTables();

    Table GetTableById(int id);

    Table GetTableByTableNumber(int tableNumber);

    Table UpdateTable(Table table);

    bool DeleteTable(int id);

    Table SetTableOccupancy(int id, bool isOccupied, int customersSeated = 0);

    List<Table> GetAvailableTables(int minimumCapacity = 1);


    // ---------------- Booking Management ---------------

    Booking AddBooking(string customerName, string phoneNumber, string email, DateTime bookingDateTime, int numberOfGuests, bool hasAllergen = false, bool allergenConsentGiven = false, int orderId = 0, string bookingComments = "", int tableNumber = 0, bool isActive = true);

    List<Booking> GetAllBookings();

    Booking GetBookingById(int id);

    List<Booking> GetBookingsByDate(DateTime date);

    List<Booking> GetActiveBookings();

    Booking UpdateBooking(Booking booking);

    bool DeleteBooking(int id);

    Booking SetBookingActiveStatus(int id, bool isActive);


    // ---------------- MenuItem Management ---------------

    MenuItem AddMenuItem(string name, string description, double price, List<Ingredient> ingredients);

    MenuItem GetMenuItemById(int id);

    List<MenuItem> GetAllMenuItems();

    bool DeleteMenuItem(int id);

    MenuItem UpdateMenuItem(MenuItem mi);


   


}

      
