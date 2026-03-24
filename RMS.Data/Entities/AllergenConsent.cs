using System.ComponentModel.DataAnnotations;


namespace RMS.Data.Entities;

    public class AllergenConsent
{
    public int Id { get; set; }
    
    public DateTime CreatedOn {get; set;} = DateTime.Now;

    public int OrderId {get; set;}

    [Required]
    public string CustomerName {get; set;}

    [Required]
    public string CustomerEmail {get; set;}

    [Required] 
    public string CustomerPhone {get; set;}

    [Required]
    public bool ConsentGiven {get; set;}

    public List<MenuItem> MenuItems {get; set;} = new List<MenuItem>();

    
}