namespace RMS.Data.Entities;

public class OrderItem
{
    public int Id { get; set; }
    public int Quantity { get; set; } = 1;
    public double UnitPrice { get; set; }
    public int MenuItemId { get; set; }
    public MenuItem MenuItem { get; set; }
}
