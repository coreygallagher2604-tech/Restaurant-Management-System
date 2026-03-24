using System.ComponentModel.DataAnnotations;


namespace RMS.Data.Entities;


    public class MenuItem
{
    public int Id { get; set; }
    public DateTime CreatedOn {get; set;} = DateTime.Now;

    public int? MenuID {get; set;}

    public string Name {get; set;}

    public double Price {get; set;}

    public string Description {get; set;}

    public string PhotoUrl { get; set; }


    public List<Ingredient> Ingredients {get; set;} = new List<Ingredient>();

}
    