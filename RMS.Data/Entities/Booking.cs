using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;


namespace RMS.Data.Entities;


    public class Booking
    {         
        
        public int Id { get; set; }
         
        [Required]
        [MaxLength(100)]
        public string CustomerName { get; set;} = "";

        public string PhoneNumber {get; set;} = "";

        public string Email {get; set;} = "";

        public DateTime BookingDateTime {get; set;}

        public int NumberOfGuests {get; set;}

        [Required]
        public bool HasAllergen {get; set;} = false;

        public bool AllergenConsentGiven {get; set;} = false;

        public int OrderId {get; set;}

        [MaxLength(500)]
        public string BookingComments {get; set;} = "";

        public int TableNumber {get; set;}

        // Comma-separated additional table numbers e.g. "3,5" — empty string means none
        public string AdditionalTableNumbers {get; set;} = "";

        public bool IsActive {get; set;} = false;

        public string Status {get; set;} = "Booked";

        public DateTime CreatedOnUtc {get; set;} = DateTime.UtcNow;

    


    }