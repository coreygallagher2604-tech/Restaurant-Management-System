using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RMS.Data.Services;
using RMS.Web.Models;

namespace RMS.Web.Controllers;

public class ReviewController : BaseController
{
    private IRestaurantService svc;

    public ReviewController()
    {
        svc = new RestaurantServiceDb();
    }

    // GET /Review/Index — staff and above can see all reviews
    [HttpGet]
    [Authorize(Roles = "admin,owner,manager,staff")]
    public IActionResult Index()
    {
        var reviews = svc.GetAllReviews();
        var vms = reviews.Select(ReviewViewModel.FromReview).ToList();
        return View(vms);
    }

    // GET /Review/Create/{orderId} — open to anyone, but order must be completed
    [HttpGet]
    public IActionResult Create(int orderId)
    {
        var order = svc.GetOrderById(orderId);

        if (order is null || !order.IsCompleted)
        {
            Alert("You can only leave a review for a completed order.", AlertType.warning);
            return RedirectToAction("Index", "Home");
        }

        var existing = svc.GetReviewByOrderId(orderId);
        if (existing is not null)
        {
            Alert("This order has already been reviewed.", AlertType.info);
            return RedirectToAction("Index", "Home");
        }

        var vm = new ReviewViewModel { OrderId = orderId };
        return View(vm);
    }

    // POST /Review/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(ReviewViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        var created = svc.AddReview(vm.OrderId, vm.CustomerName, vm.Stars, vm.Comment);

        if (created is not null)
        {
            Alert("Thank you for your review!", AlertType.success);
            return RedirectToAction("Index", "Home");
        }

        Alert("Review could not be saved. The order may not be completed, or has already been reviewed.", AlertType.warning);
        return View(vm);
    }

    // GET /Review/Delete/{id} — admin and owner only
    [HttpGet]
    [Authorize(Roles = "admin,owner")]
    public IActionResult Delete(int id)
    {
        var review = svc.GetReviewById(id);

        if (review is null)
        {
            Alert($"Review {id} not found.", AlertType.warning);
            return RedirectToAction(nameof(Index));
        }

        return View(ReviewViewModel.FromReview(review));
    }

    // POST /Review/DeleteConfirm/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin,owner")]
    public IActionResult DeleteConfirm(int id)
    {
        svc.DeleteReview(id);
        Alert("Review deleted.", AlertType.success);
        return RedirectToAction(nameof(Index));
    }
}
