using System.ComponentModel.DataAnnotations;
using RMS.Data.Entities;

namespace RMS.Web.Models;

public class TableViewModel
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Table Number")]
    [Range(1, 50)]
    public int TableNumber { get; set; }

    [Required]
    [Display(Name = "Seating Capacity")]
    [Range(2, 20)]
    public int SeatingCapacity { get; set; }

    [Display(Name = "Customers Seated")]
    [Range(0, 20)]
    public int CustomersSeated { get; set; } = 0;

    [Display(Name = "Occupied")]
    public bool IsOccupied { get; set; } = false;

    public bool Active { get; set; } = true;

    // Set by TableController.Index — not stored on the entity
    public bool IsReserved { get; set; } = false;
    public DateTime? ReservedAt { get; set; } = null;

    public string StatusDisplay =>
        !Active    ? "Inactive" :
        IsOccupied ? "Occupied" :
        IsReserved ? "Reserved" :
                     "Available";

    public Table ToTable()
    {
        var table = new Table();
        table.Id = Id;
        table.TableNumber = TableNumber;
        table.SeatingCapacity = SeatingCapacity;
        table.CustomersSeated = CustomersSeated;
        table.IsOccupied = IsOccupied;
        table.Active = Active;
        return table;
    }

    public static TableViewModel FromTable(Table t)
    {
        var vm = new TableViewModel();
        vm.Id = t.Id;
        vm.TableNumber = t.TableNumber;
        vm.SeatingCapacity = t.SeatingCapacity;
        vm.CustomersSeated = t.CustomersSeated;
        vm.IsOccupied = t.IsOccupied;
        vm.Active = t.Active;
        return vm;
    }
}
