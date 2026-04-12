using System.ComponentModel.DataAnnotations;
using RMS.Data.Entities;

namespace RMS.Web.Models;

public class ReviewViewModel
{
    public int Id { get; set; }

    [Display(Name = "Order #")]
    public int OrderId { get; set; }

    [Required(ErrorMessage = "Your name is required.")]
    [Display(Name = "Your Name")]
    public string CustomerName { get; set; } = "";

    [Required(ErrorMessage = "Please select a star rating.")]
    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5 stars.")]
    [Display(Name = "Rating")]
    public int Stars { get; set; }

    [Display(Name = "Comment")]
    [MaxLength(1000)]
    public string Comment { get; set; } = "";

    public DateTime CreatedOn { get; set; }

    public static ReviewViewModel FromReview(Review r)
    {
        return new ReviewViewModel
        {
            Id = r.Id,
            OrderId = r.OrderId,
            CustomerName = r.CustomerName,
            Stars = r.Stars,
            Comment = r.Comment,
            CreatedOn = r.CreatedOn
        };
    }
}
