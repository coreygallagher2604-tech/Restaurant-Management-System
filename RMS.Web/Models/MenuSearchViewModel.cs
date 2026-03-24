using RMS.Data.Entities;

namespace RMS.Web.Models;

public class MenuSearchViewModel
{
    public string Query { get; set; } = "";
    public int Range { get; set; } = 10;
    public IList<Menu> Menus { get; set; } = new List<Menu>();
}
