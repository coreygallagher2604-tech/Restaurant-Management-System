using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;


namespace RMS.Data.Entities;

    public class Order
{
    public int Id { get; set; }
    
    public DateTime CreatedOn {get; set;} = DateTime.Now;

    public List<MenuItem> MenuItems {get; set;} = new List<MenuItem>();

    public double totalCost {get; set;}

    public Table Table {get; set;}


    List<AllergenConsent> AllergenConsents {get; set;} = new List<AllergenConsent>();

    public bool IsCompleted {get; set;} = false;

    public bool IsVoid {get; set;} = false;

    [Range(0, 100)]
    public int discount {get; set;} = 0;

    public double FinalPrice {get; set;} = 0;

    public int AllergyCountRequired {get; set;} = 0;
    

    
}