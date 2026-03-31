using System.ComponentModel.DataAnnotations;
using RMS.Data.Entities;

namespace RMS.Web.Models;

public class TableViewModel
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Table Number")]
    [Range(1, 999)]
    public int TableNumber { get; set; }

    [Required]
    [Display(Name = "Seating Capacity")]
    [Range(1, 20)]
    public int SeatingCapacity { get; set; }

    [Display(Name = "Customers Seated")]
    [Range(0, 20)]
    public int CustomersSeated { get; set; } = 0;

    [Display(Name = "Occupied")]
    public bool IsOccupied { get; set; } = false;

    public bool Active { get; set; } = true;

    public string StatusDisplay => IsOccupied ? "Occupied" : "Available";

    public Table ToTable() => new Table
    {
        Id = Id,
        TableNumber = TableNumber,
        SeatingCapacity = SeatingCapacity,
        CustomersSeated = CustomersSeated,
        IsOccupied = IsOccupied,
        Active = Active
    };

    public static TableViewModel FromTable(Table t) => new TableViewModel
    {
        Id = t.Id,
        TableNumber = t.TableNumber,
        SeatingCapacity = t.SeatingCapacity,
        CustomersSeated = t.CustomersSeated,
        IsOccupied = t.IsOccupied,
        Active = t.Active
    };
}
