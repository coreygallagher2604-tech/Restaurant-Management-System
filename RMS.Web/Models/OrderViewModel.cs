using System.ComponentModel.DataAnnotations;
using RMS.Data.Entities;

namespace RMS.Web.Models;

public class OrderViewModel
{
    public int Id { get; set; }

    public DateTime CreatedOn { get; set; }

    public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public Dictionary<int, int> ItemQuantities { get; set; } = new Dictionary<int, int>();

    public List<MenuItem> AvailableMenuItems { get; set; } = new List<MenuItem>();

    [Display(Name = "Table")]
    public int? TableId { get; set; }

    public List<Table> AvailableTables { get; set; } = new List<Table>();

    public double TotalCost { get; set; }

    public bool IsCompleted { get; set; } = false;

    public bool IsVoid { get; set; } = false;

    public string CourseStatus { get; set; } = "NotStarted";

    public int AllergyCountRequired { get; set; } = 0;

    public int AllergenConsentCount { get; set; } = 0;

    public bool NeedsAllergenConsent => AllergyCountRequired > 0 && AllergenConsentCount < AllergyCountRequired;

    public string StatusDisplay
    {
        get
        {
            if (IsVoid) { return "Void"; }
            if (IsCompleted) { return "Completed"; }
            return "Open";
        }
    }

    public string StatusBadgeClass
    {
        get
        {
            if (IsVoid) { return "bg-secondary"; }
            if (IsCompleted) { return "bg-success"; }
            return "bg-warning text-dark";
        }
    }

    public static OrderViewModel FromOrder(Order o)
    {
        var vm = new OrderViewModel();
        vm.Id = o.Id;
        vm.CreatedOn = o.CreatedOn;
        vm.OrderItems = o.OrderItems ?? new List<OrderItem>();
        vm.ItemQuantities = (o.OrderItems ?? new List<OrderItem>())
            .ToDictionary(oi => oi.MenuItemId, oi => oi.Quantity);
        vm.TableId = o.Table?.Id;
        vm.TotalCost = o.totalCost;
        vm.IsCompleted = o.IsCompleted;
        vm.IsVoid = o.IsVoid;
        vm.CourseStatus = o.CourseStatus;
        vm.AllergyCountRequired = o.AllergyCountRequired;
        vm.AllergenConsentCount = o.AllergenConsents?.Count ?? 0;
        return vm;
    }
}
