using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;


namespace RMS.Data.Entities;


    public class Menu
    {         
        
        public int Id { get; set; }
         
        [Required]
        [MaxLength(100)]
        public string Name { get; set;} = "";

        public string Type {get; set;} = "";

        [Required]
        [MaxLength(500)]
        public string Description { get; set;} = "";

        public bool IsActive {get; set;} = true;

        public DateTime CreatedOn {get; set;} = DateTime.UtcNow;

        public List<MenuItem> MenuItems {get; set;} = new List<MenuItem>();


    }
