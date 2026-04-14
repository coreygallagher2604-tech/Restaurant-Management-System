using System.ComponentModel.DataAnnotations;
using RMS.Data.Entities;

namespace RMS.Web.Models;

public class AllergenConsentViewModel
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    [Required(ErrorMessage = "Customer name is required.")]
    [MinLength(2, ErrorMessage = "Customer name must be at least 2 characters.")]
    [Display(Name = "Customer Name")]
    public string CustomerName { get; set; } = "";

    [Required(ErrorMessage = "Email is required.")]
    [RegularExpression(
        @"^[^@\s]+@[^@\s]+\.[^@\s]{2,}$",
        ErrorMessage = "Enter a valid email address (e.g. name@example.com).")]
    [Display(Name = "Customer Email")]
    public string CustomerEmail { get; set; } = "";

    [Required(ErrorMessage = "Phone number is required.")]
    [RegularExpression(
        @"^(\+353|\+44|028)[0-9]{7,10}$|^0[0-9]{8,9}$",
        ErrorMessage = "Enter a valid Irish (+353) or UK (+44) phone number.")]
    [Display(Name = "Phone Number")]
    public string CustomerPhone { get; set; } = "";

    [Display(Name = "Consent Given")]
    public bool ConsentGiven { get; set; } = false;

    public DateTime CreatedOn { get; set; }

    public List<MenuItem> MenuItems { get; set; } = new List<MenuItem>();

    // For Create form — available orders and menu items
    public List<Order> AvailableOrders { get; set; } = new List<Order>();
    public List<MenuItem> AvailableMenuItems { get; set; } = new List<MenuItem>();
    public List<int> SelectedMenuItemIds { get; set; } = new List<int>();

    public static AllergenConsentViewModel FromConsent(AllergenConsent c)
    {
        var vm = new AllergenConsentViewModel();
        vm.Id = c.Id;
        vm.OrderId = c.OrderId;
        vm.CustomerName = c.CustomerName;
        vm.CustomerEmail = c.CustomerEmail;
        vm.CustomerPhone = c.CustomerPhone;
        vm.ConsentGiven = c.ConsentGiven;
        vm.CreatedOn = c.CreatedOn;
        vm.MenuItems = c.MenuItems;
        return vm;
    }
}
