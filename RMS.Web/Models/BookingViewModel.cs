using System.ComponentModel.DataAnnotations;
using RMS.Data.Entities;

namespace RMS.Web.Models;

public class BookingViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Customer name is required.")]
    [MaxLength(100)]
    public string CustomerName { get; set; } = "";

    [Required(ErrorMessage = "Phone number is required.")]
    public string PhoneNumber { get; set; } = "";

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Booking date and time is required.")]
    public DateTime BookingDateTime { get; set; } = DateTime.Now;

    [Range(1, 50, ErrorMessage = "Number of guests must be at least 1.")]
    public int NumberOfGuests { get; set; } = 1;

    public bool HasAllergen { get; set; } = false;

    public bool AllergenConsentGiven { get; set; } = false;

    public int OrderId { get; set; }

    [MaxLength(500)]
    public string BookingComments { get; set; } = "";

    public int TableNumber { get; set; }

    public bool IsActive { get; set; } = true;

    public static BookingViewModel FromBooking(Booking b)
    {
        var vm = new BookingViewModel();
        vm.Id = b.Id;
        vm.CustomerName = b.CustomerName;
        vm.PhoneNumber = b.PhoneNumber;
        vm.Email = b.Email;
        vm.BookingDateTime = b.BookingDateTime;
        vm.NumberOfGuests = b.NumberOfGuests;
        vm.HasAllergen = b.HasAllergen;
        vm.AllergenConsentGiven = b.AllergenConsentGiven;
        vm.OrderId = b.OrderId;
        vm.BookingComments = b.BookingComments;
        vm.TableNumber = b.TableNumber;
        vm.IsActive = b.IsActive;
        return vm;
    }

    public Booking ToBooking()
    {
        var b = new Booking();
        b.Id = Id;
        b.CustomerName = CustomerName;
        b.PhoneNumber = PhoneNumber;
        b.Email = Email;
        b.BookingDateTime = BookingDateTime;
        b.NumberOfGuests = NumberOfGuests;
        b.HasAllergen = HasAllergen;
        b.AllergenConsentGiven = AllergenConsentGiven;
        b.OrderId = OrderId;
        b.BookingComments = BookingComments;
        b.TableNumber = TableNumber;
        b.IsActive = IsActive;
        return b;
    }
}
