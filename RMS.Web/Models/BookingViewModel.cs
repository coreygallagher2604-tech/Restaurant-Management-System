using System.ComponentModel.DataAnnotations;
using RMS.Data.Entities;

namespace RMS.Web.Models;

public class BookingViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Customer name is required.")]
    [MinLength(2, ErrorMessage = "Customer name must be at least 2 characters.")]
    [MaxLength(100)]
    [Display(Name = "Customer Name")]
    public string CustomerName { get; set; } = "";

    [Required(ErrorMessage = "Phone number is required.")]
    [RegularExpression(
        @"^(\+353|\+44|028)[0-9]{7,10}$|^0[0-9]{8,9}$",
        ErrorMessage = "Enter a valid Irish (+353) or UK (+44) phone number.")]
    [Display(Name = "Phone Number")]
    public string PhoneNumber { get; set; } = "";

    [Required(ErrorMessage = "Email is required.")]
    [RegularExpression(
        @"^[^@\s]+@[^@\s]+\.[^@\s]{2,}$",
        ErrorMessage = "Enter a valid email address (e.g. name@example.com).")]
    [Display(Name = "Email Address")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Booking date and time is required.")]
    [Display(Name = "Booking Date & Time")]
    public DateTime BookingDateTime { get; set; } = DateTime.Now;

    [Range(1, 50, ErrorMessage = "Number of guests must be at least 1.")]
    [Display(Name = "Number of Guests")]
    public int NumberOfGuests { get; set; } = 1;

    [Display(Name = "Do any of the guests have an allergy?")]
    public bool HasAllergen { get; set; } = false;

    public bool AllergenConsentGiven { get; set; } = false;

    public int OrderId { get; set; }

    [MaxLength(500)]
    public string BookingComments { get; set; } = "";

    public int TableNumber { get; set; }

    // Comma-separated additional table numbers stored as string; parsed for capacity checks
    public string AdditionalTableNumbers { get; set; } = "";

    // Parsed list \u2014 used in the view to pre-check checkboxes
    public List<int> AdditionalTableNumbersList =>
        string.IsNullOrWhiteSpace(AdditionalTableNumbers)
            ? new List<int>()
            : AdditionalTableNumbers.Split(',', System.StringSplitOptions.RemoveEmptyEntries)
                                     .Select(int.Parse).ToList();

    public bool IsActive { get; set; } = true;

    public string Status { get; set; } = "Booked";

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
        vm.AdditionalTableNumbers = b.AdditionalTableNumbers ?? "";
        vm.IsActive = b.IsActive;
        vm.Status = b.Status;
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
        b.AdditionalTableNumbers = AdditionalTableNumbers ?? "";
        b.IsActive = IsActive;
        b.Status = Status;
        return b;
    }
}
