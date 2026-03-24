using System.ComponentModel.DataAnnotations;


namespace RMS.Data.Entities;

    public class Table
{
    public int Id { get; set; }
    
    public DateTime CreatedOn {get; set;} = DateTime.Now;

    public int TableNumber {get; set;}

    public List<Order> Orders {get; set;} = new List<Order>();

    public bool IsOccupied {get; set;} = false;

    public int OrderNumber {get; set;}

    public int SeatingCapacity {get; set;}

    public int CustomersSeated { get; set; } = 0;    

    public bool Active {get; set;} = true;
}
