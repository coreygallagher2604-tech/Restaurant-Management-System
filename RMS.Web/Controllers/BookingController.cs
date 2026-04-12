using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RMS.Data.Entities;
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

    // GET /Booking/CreateBooking — open to all users including guests and anonymous
    // Accepts optional query params from the Home page booking partial
    [HttpGet]
    public IActionResult CreateBooking(string bookingDate = null, string bookingTime = null, int? numberOfGuests = null)
    {
        DateTime bookingDateTime;

        // Try to combine the date and time values passed from the Home partial
        if (!string.IsNullOrEmpty(bookingDate) && !string.IsNullOrEmpty(bookingTime)
            && DateTime.TryParse($"{bookingDate} {bookingTime}", out var parsed))
        {
            bookingDateTime = parsed;
        }
        else
        {
            // Fall back to the next 15-minute boundary
            var now = DateTime.Now;
            var minutes = 15 * (int)Math.Ceiling(now.Minute / 15.0);
            bookingDateTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0).AddMinutes(minutes);
        }

        int guestCount = numberOfGuests ?? 1;

        // Check if the requested slot is available — if not, find the next one
        bookingDateTime = FindNextAvailableSlot(bookingDateTime, guestCount);

        var vm = new BookingViewModel
        {
            BookingDateTime = bookingDateTime,
            NumberOfGuests = guestCount
        };

        // Pre-fill name and email for logged-in users
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            vm.CustomerName = User.FindFirstValue(ClaimTypes.Name) ?? "";
            vm.Email = User.FindFirstValue(ClaimTypes.Email) ?? "";
        }

        return View("Create", vm);
    }

    // Finds the next available 15-minute slot on the same day from the requested time.
    // Looks up to 8 hours ahead. Returns the original time if no slot is found
    // (the POST will reject it with a proper message).
    private DateTime FindNextAvailableSlot(DateTime requested, int guestCount)
    {
        int slotMinutes = 135;
        DateTime candidate = requested;

        // Check up to 32 slots (8 hours in 15-min increments)
        for (int i = 0; i < 32; i++)
        {
            DateTime candidateEnd = candidate.AddMinutes(slotMinutes);

            // Get existing bookings on this day
            List<Booking> existingBookings = new List<Booking>();
            foreach (Booking b in svc.GetAllBookings())
            {
                if (b.Status != "Cancelled" && b.BookingDateTime.Date == candidate.Date)
                {
                    existingBookings.Add(b);
                }
            }

            // Find a table with no conflict at this candidate time
            bool slotFound = false;
            List<Table> suitableTables = new List<Table>();
            foreach (Table t in svc.GetAllTables())
            {
                if (t.Active && t.SeatingCapacity >= guestCount)
                {
                    suitableTables.Add(t);
                }
            }

            foreach (Table table in suitableTables.OrderBy(t => t.SeatingCapacity))
            {
                bool hasConflict = false;
                foreach (Booking existing in existingBookings)
                {
                    if (existing.TableNumber == table.TableNumber)
                    {
                        DateTime existingEnd = existing.BookingDateTime.AddMinutes(slotMinutes);
                        if (candidate < existingEnd && existing.BookingDateTime < candidateEnd)
                        {
                            hasConflict = true;
                            break;
                        }
                    }
                }

                if (!hasConflict)
                {
                    slotFound = true;
                    break;
                }
            }

            if (slotFound)
            {
                return candidate;
            }

            candidate = candidate.AddMinutes(15);
        }

        return requested; // Nothing found — POST will handle the rejection
    }

    // POST /Booking/CreateBooking — open to all users including guests and anonymous
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CreateBooking(BookingViewModel vm, string bookingDatePart, string bookingTimePart)
    {
        // Combine the separate date and time inputs into BookingDateTime
        if (!string.IsNullOrEmpty(bookingDatePart) && !string.IsNullOrEmpty(bookingTimePart))
        {
            string combinedString = bookingDatePart + " " + bookingTimePart;
            DateTime parsedDateTime;

            if (DateTime.TryParse(combinedString, out parsedDateTime))
            {
                vm.BookingDateTime = parsedDateTime;
                ModelState.Remove("BookingDateTime");
            }
        }

        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        // Each booking occupies a 2-hour slot plus 15 minutes for table turnaround
        int slotMinutes = 135;
        DateTime requestedStart = vm.BookingDateTime;
        DateTime requestedEnd = requestedStart.AddMinutes(slotMinutes);

        // Get all bookings on the same day that are not cancelled
        List<Booking> existingBookings = new List<Booking>();
        foreach (Booking b in svc.GetAllBookings())
        {
            if (b.Status != "Cancelled" && b.BookingDateTime.Date == requestedStart.Date)
            {
                existingBookings.Add(b);
            }
        }

        // Find the smallest available table that fits the party and has no time clash
        List<Table> suitableTables = new List<Table>();
        foreach (Table t in svc.GetAllTables())
        {
            if (t.Active && t.SeatingCapacity >= vm.NumberOfGuests)
            {
                suitableTables.Add(t);
            }
        }
        suitableTables = suitableTables.OrderBy(t => t.SeatingCapacity).ToList();

        Table assignedTable = null;
        foreach (Table table in suitableTables)
        {
            bool hasConflict = false;
            foreach (Booking existing in existingBookings)
            {
                if (existing.TableNumber == table.TableNumber)
                {
                    DateTime existingStart = existing.BookingDateTime;
                    DateTime existingEnd = existingStart.AddMinutes(slotMinutes);

                    // Two slots clash if one starts before the other ends
                    if (requestedStart < existingEnd && existingStart < requestedEnd)
                    {
                        hasConflict = true;
                        break;
                    }
                }
            }

            if (!hasConflict)
            {
                assignedTable = table;
                break;
            }
        }

        if (assignedTable == null)
        {
            ModelState.AddModelError("", "Sorry, no tables are available for that date, time and party size. The next available slot may be later in the day.");
            return View("Create", vm);
        }

        int assignedTableNumber = assignedTable.TableNumber;

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
            assignedTableNumber,
            true  // IsActive is always true on creation — staff manage this via Edit
        );

        if (created is not null)
        {
            Alert($"Booking for '{created.CustomerName}' added.", AlertType.success);
            return RedirectToAction(nameof(Index));
        }

        Alert("Booking could not be created.", AlertType.warning);
        return View("Create", vm);
    }

    // GET /Booking/Edit/{id}
    [HttpGet]
    [Authorize(Roles = "admin,owner,manager,staff")]
    public IActionResult Edit(int id)
    {
        var booking = svc.GetBookingById(id);

        if (booking is null)
        {
            Alert($"Booking {id} not found.", AlertType.warning);
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Tables = svc.GetAllTables().Where(t => t.Active).OrderBy(t => t.TableNumber).ToList();
        return View(BookingViewModel.FromBooking(booking));
    }

    // POST /Booking/Edit/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin,owner,manager,staff")]
    public IActionResult Edit(int id, BookingViewModel vm, string bookingDatePart, string bookingTimePart, List<int> additionalTableNumbers)
    {
        // Combine the separate date and time inputs into BookingDateTime
        if (!string.IsNullOrEmpty(bookingDatePart) && !string.IsNullOrEmpty(bookingTimePart))
        {
            string combinedString = bookingDatePart + " " + bookingTimePart;
            DateTime parsedDateTime;

            if (DateTime.TryParse(combinedString, out parsedDateTime))
            {
                vm.BookingDateTime = parsedDateTime;
                ModelState.Remove("BookingDateTime");
            }
        }

        // Store additional tables as comma-separated string
        vm.AdditionalTableNumbers = additionalTableNumbers != null && additionalTableNumbers.Any()
            ? string.Join(",", additionalTableNumbers)
            : "";

        // Validate: primary table must be assigned
        if (vm.TableNumber <= 0)
        {
            ModelState.AddModelError("TableNumber", "A table must be selected.");
        }
        else
        {
            var allTables = svc.GetAllTables().ToList();
            var primaryTable = allTables.FirstOrDefault(t => t.TableNumber == vm.TableNumber);
            if (primaryTable == null)
            {
                ModelState.AddModelError("TableNumber", $"Table {vm.TableNumber} does not exist.");
            }
            else
            {
                // Calculate combined seating capacity across all selected tables
                int totalCapacity = primaryTable.SeatingCapacity;
                foreach (var tNum in additionalTableNumbers ?? new List<int>())
                {
                    var extra = allTables.FirstOrDefault(t => t.TableNumber == tNum);
                    if (extra != null) totalCapacity += extra.SeatingCapacity;
                }

                if (totalCapacity < vm.NumberOfGuests)
                {
                    ModelState.AddModelError("TableNumber", $"Selected tables seat {totalCapacity} but the booking is for {vm.NumberOfGuests} guest(s).");
                }
            }
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Tables = svc.GetAllTables().Where(t => t.Active).OrderBy(t => t.TableNumber).ToList();
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
        ViewBag.Tables = svc.GetAllTables().Where(t => t.Active).OrderBy(t => t.TableNumber).ToList();
        return View(vm);
    }

    // GET /Booking/Delete/{id}
    [HttpGet]
    [Authorize(Roles = "admin,owner,manager,staff")]
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
    [Authorize(Roles = "admin,owner,manager,staff")]
    public IActionResult DeleteConfirm(int id)
    {
        var booking = svc.GetBookingById(id);
        if (booking is null)
        {
            Alert("Booking not found.", AlertType.warning);
            return RedirectToAction(nameof(Index));
        }

        // Block delete if the booking has an active (non-completed, non-void) order
        if (booking.OrderId > 0)
        {
            var order = svc.GetOrderById(booking.OrderId);
            if (order != null && !order.IsCompleted && !order.IsVoid)
            {
                Alert("Cannot delete this booking \u2014 the customer has an active order. Complete or void the order first.", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }
        }

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
    [Authorize(Roles = "admin,owner,manager,staff")]
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
