using RMS.Data.Entities;

namespace RMS.Web.Models;

public class MenuItemSearchViewModel
{
    public string Query { get; set; } = "";
    public IList<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
}
