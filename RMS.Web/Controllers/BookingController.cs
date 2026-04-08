using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RMS.Data.Services;
using RMS.Web.Models;

namespace RMS.Web.Controllers;

public class BookingController : BaseController
{
    private IRestaurantService svc;

    public BookingController()
    {
        svc = new RestaurantServiceDb();
    }

    // GET /Booking/Index
    [HttpGet]
    public IActionResult Index()
    {
        var bookings = svc.GetAllBookings();
        var vms = bookings.Select(BookingViewModel.FromBooking).ToList();
        return View(vms);
    }

    // GET /Booking/Details/{id}
    [HttpGet]
    public IActionResult Details(int id)
    {
        var booking = svc.GetBookingById(id);

        if (booking is null)
        {
            Alert($"Booking {id} not found.", AlertType.warning);
            return RedirectToAction(nameof(Index));
        }

        return View(BookingViewModel.FromBooking(booking));
    }

    // GET /Booking/Create
    [HttpGet]
    [Authorize(Roles = "admin,authenticated")]
    public IActionResult Create()
    {
        return View(new BookingViewModel());
    }

    // POST /Booking/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin,authenticated")]
    public IActionResult Create(BookingViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        var created = svc.AddBooking(
            vm.CustomerName,
            vm.PhoneNumber,
            vm.Email,
            vm.BookingDateTime,
            vm.NumberOfGuests,
            vm.HasAllergen,
            vm.AllergenConsentGiven,
            vm.OrderId,
            vm.BookingComments,
            vm.TableNumber,
            vm.IsActive
        );

        if (created is not null)
        {
            Alert($"Booking for '{created.CustomerName}' added.", AlertType.success);
            return RedirectToAction(nameof(Index));
        }

        Alert("Booking could not be created.", AlertType.warning);
        return View(vm);
    }

    // GET /Booking/Edit/{id}
    [HttpGet]
    [Authorize(Roles = "admin,authenticated")]
    public IActionResult Edit(int id)
    {
        var booking = svc.GetBookingById(id);

        if (booking is null)
        {
            Alert($"Booking {id} not found.", AlertType.warning);
            return RedirectToAction(nameof(Index));
        }

        return View(BookingViewModel.FromBooking(booking));
    }

    // POST /Booking/Edit/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin,authenticated")]
    public IActionResult Edit(int id, BookingViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        var booking = vm.ToBooking();
        booking.Id = id;
        var updated = svc.UpdateBooking(booking);

        if (updated is not null)
        {
            Alert($"Booking for '{updated.CustomerName}' updated.", AlertType.success);
            return RedirectToAction(nameof(Index));
        }

        Alert("Booking could not be updated.", AlertType.warning);
        return View(vm);
    }

    // GET /Booking/Delete/{id}
    [HttpGet]
    [Authorize(Roles = "admin,authenticated")]
    public IActionResult Delete(int id)
    {
        var booking = svc.GetBookingById(id);

        if (booking is null)
        {
            Alert($"Booking {id} not found.", AlertType.warning);
            return RedirectToAction(nameof(Index));
        }

        return View(BookingViewModel.FromBooking(booking));
    }

    // POST /Booking/DeleteConfirm/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin,authenticated")]
    public IActionResult DeleteConfirm(int id)
    {
        var deleted = svc.DeleteBooking(id);

        if (deleted)
        {
            Alert("Booking deleted.", AlertType.success);
        }
        else
        {
            Alert("Booking could not be deleted.", AlertType.warning);
        }

        return RedirectToAction(nameof(Index));
    }

    // POST /Booking/SetActive/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin,authenticated")]
    public IActionResult SetActive(int id, bool isActive)
    {
        var updated = svc.SetBookingActiveStatus(id, isActive);

        if (updated is not null)
        {
            Alert($"Booking status updated.", AlertType.success);
        }
        else
        {
            Alert("Booking status could not be updated.", AlertType.warning);
        }

        return RedirectToAction(nameof(Index));
    }
}
