using System.ComponentModel.DataAnnotations;


namespace RMS.Data.Entities;


    public class Ingredient
{
    public int Id { get; set; }

    [Required]
    public bool Allergen {get; set;} = false;

    public DateTime CreatedOn {get; set;} = DateTime.Now;

    public DateTime LastUpdatedOn {get;} = DateTime.Now;

    [Required] 
    public string Name {get; set;}

    [Required]
    public string AllergenInfo {get; set;}

    
}