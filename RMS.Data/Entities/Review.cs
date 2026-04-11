using System.ComponentModel.DataAnnotations;

namespace RMS.Data.Entities;

public class Review
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    [Required]
    public string CustomerName { get; set; } = "";

    [Range(1, 5)]
    public int Stars { get; set; }

    [MaxLength(1000)]
    public string Comment { get; set; } = "";

    public DateTime CreatedOn { get; set; } = DateTime.Now;
}
