using System.ComponentModel.DataAnnotations;
using RMS.Data.Entities;

namespace RMS.Web.Models;

public class AllergenConsentViewModel
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    [Required(ErrorMessage = "Customer name is required.")]
    public string CustomerName { get; set; } = "";

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress]
    public string CustomerEmail { get; set; } = "";

    [Required(ErrorMessage = "Phone number is required.")]
    public string CustomerPhone { get; set; } = "";

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
