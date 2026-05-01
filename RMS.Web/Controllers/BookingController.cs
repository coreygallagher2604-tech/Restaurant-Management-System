using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RMS.Data.Entities;
using RMS.Data.Services;
using RMS.Web.Models;

namespace RMS.Web.Controllers;

[Authorize(Roles = "admin,owner,manager,staff")]
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
        var bookings = svc.GetAllBookings()
            .Where(b => b.Status != "Cancelled")
            .OrderBy(b => b.Status == "Seated" ? 0 : b.Status == "Booked" ? 1 : 2)
            .ThenBy(b => b.BookingDateTime)
            .ToList();
        var vms = bookings.Select(BookingViewModel.FromBooking).ToList();
        return View(vms);
    }

    // GET /Booking/Cancelled — cancelled bookings, managers and above only
    [HttpGet]
    [Authorize(Roles = "admin,owner,manager")]
    public IActionResult Cancelled()
    {
        var bookings = svc.GetAllBookings()
            .Where(b => b.Status == "Cancelled")
            .OrderByDescending(b => b.BookingDateTime)
            .ToList();
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

    // GET /Booking/Confirmation/{id} — open to all users so guests can see their booking confirmation
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Confirmation(int id)
    {
        var booking = svc.GetBookingById(id);
        if (booking is null)
        {
            return RedirectToAction("Index", "Home");
        }
        return View(BookingViewModel.FromBooking(booking));
    }

    // GET /Booking/CreateBooking — open to all users including guests and anonymous
    // Accepts optional query params from the Home page booking partial
    [HttpGet]
    [AllowAnonymous]
    public IActionResult CreateBooking(string bookingDate = null, string bookingTime = null, int? numberOfGuests = null)
    {
        DateTime bookingDateTime;

        // Calculate the next 15-minute boundary from now — never allow a past slot
        var now = DateTime.Now;
        var nextSlot = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0)
            .AddMinutes(15 * (int)Math.Ceiling(now.Minute / 15.0));

        // Try to combine the date and time values passed from the Home partial
        if (!string.IsNullOrEmpty(bookingDate) && !string.IsNullOrEmpty(bookingTime)
            && DateTime.TryParse($"{bookingDate} {bookingTime}", out var parsed))
        {
            bookingDateTime = parsed < nextSlot ? nextSlot : parsed;
        }
        else
        {
            bookingDateTime = nextSlot;
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
    [AllowAnonymous]
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

        // Reject bookings in the past
        if (vm.BookingDateTime < DateTime.Now)
        {
            ModelState.AddModelError("", $"Booking time must be in the future. The earliest available slot is after {DateTime.Now:h:mm tt}.");
            return View("Create", vm);
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
            vm.BookingComments,
            assignedTableNumber,
            true  // IsActive is always true on creation — staff manage this via Edit
        );

        if (created is not null)
        {
            return RedirectToAction(nameof(Confirmation), new { id = created.Id });
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

        var allTables = svc.GetAllTables().Where(t => t.Active).OrderBy(t => t.TableNumber).ToList();
        ViewBag.Tables = allTables;

        // Calculate which table numbers are already taken by OTHER bookings in the same 135-minute slot
        var slotStart = booking.BookingDateTime;
        var slotEnd = slotStart.AddMinutes(135);
        var takenTableNumbers = new HashSet<int>();
        foreach (var b in svc.GetAllBookings())
        {
            if (b.Id == id || b.Status == "Cancelled") continue;
            var bEnd = b.BookingDateTime.AddMinutes(135);
            if (b.BookingDateTime < slotEnd && bEnd > slotStart)
            {
                if (b.TableNumber > 0) takenTableNumbers.Add(b.TableNumber);
                if (!string.IsNullOrWhiteSpace(b.AdditionalTableNumbers))
                    foreach (var n in b.AdditionalTableNumbers.Split(',', StringSplitOptions.RemoveEmptyEntries))
                        if (int.TryParse(n.Trim(), out var tn)) takenTableNumbers.Add(tn);
            }
        }
        ViewBag.TakenTableNumbers = takenTableNumbers;

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
                    bool canOverride = User.IsInRole("admin") || User.IsInRole("owner") || User.IsInRole("manager");
                    if (!canOverride)
                    {
                        ModelState.AddModelError("TableNumber", $"Selected tables seat {totalCapacity} but the booking is for {vm.NumberOfGuests} guest(s). A manager or owner must approve this override.");
                    }
                    // managers/owners/admins proceed — capacity warning shown after save
                }
            }
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Tables = svc.GetAllTables().Where(t => t.Active).OrderBy(t => t.TableNumber).ToList();
            ViewBag.TakenTableNumbers = new HashSet<int>();
            return View(vm);
        }

        // Capture original tables before updating, so we can sync occupancy
        var originalBooking = svc.GetBookingById(id);
        int originalTableNumber = originalBooking?.TableNumber ?? 0;
        var originalAdditional = (originalBooking?.AdditionalTableNumbers ?? "")
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => int.TryParse(s.Trim(), out var n) ? n : 0)
            .Where(n => n > 0).ToHashSet();
        var newAdditional = (vm.AdditionalTableNumbers ?? "")
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => int.TryParse(s.Trim(), out var n) ? n : 0)
            .Where(n => n > 0).ToHashSet();
        bool wasSeated = originalBooking?.Status == "Seated";

        // Calculate total capacity for override warning (after ModelState check)
        var allTablesForCapacity = svc.GetAllTables().ToList();
        int updatedTotalCapacity = allTablesForCapacity.FirstOrDefault(t => t.TableNumber == vm.TableNumber)?.SeatingCapacity ?? 0;
        foreach (var tNum in additionalTableNumbers ?? new List<int>())
        {
            var extra = allTablesForCapacity.FirstOrDefault(t => t.TableNumber == tNum);
            if (extra != null) updatedTotalCapacity += extra.SeatingCapacity;
        }

        var booking = vm.ToBooking();
        booking.Id = id;
        var updated = svc.UpdateBooking(booking);

        if (updated is not null)
        {
            // If the primary table changed on a seated booking, sync occupancy
            if (wasSeated && updated.TableNumber != originalTableNumber)
            {
                if (originalTableNumber > 0)
                {
                    var oldTable = svc.GetTableByTableNumber(originalTableNumber);
                    if (oldTable != null) svc.SetTableOccupancy(oldTable.Id, false, 0);
                }
                if (updated.TableNumber > 0)
                {
                    var newTable = svc.GetTableByTableNumber(updated.TableNumber);
                    if (newTable != null) svc.SetTableOccupancy(newTable.Id, true, updated.NumberOfGuests);
                }
                // Move any active order to the new table
                if (originalTableNumber > 0 && updated.TableNumber > 0)
                {
                    var oldTable = svc.GetTableByTableNumber(originalTableNumber);
                    if (oldTable != null)
                    {
                        var order = svc.GetOrdersByTableId(oldTable.Id)
                            .FirstOrDefault(o => !o.IsCompleted && !o.IsVoid);
                        var newTable = svc.GetTableByTableNumber(updated.TableNumber);
                        if (order != null && newTable != null)
                        {
                            order.Table = newTable;
                            svc.UpdateOrder(order);
                        }
                    }
                }
            }

            // Sync additional tables for seated bookings
            if (wasSeated)
            {
                // Free tables that were removed
                foreach (var tn in originalAdditional.Except(newAdditional))
                {
                    var t = svc.GetTableByTableNumber(tn);
                    if (t != null) svc.SetTableOccupancy(t.Id, false, 0);
                }
                // Occupy tables that were added
                foreach (var tn in newAdditional.Except(originalAdditional))
                {
                    var t = svc.GetTableByTableNumber(tn);
                    if (t != null) svc.SetTableOccupancy(t.Id, true, updated.NumberOfGuests);
                }
            }

            // Warn if capacity exceeded (manager/owner/admin override)
            if (updated.NumberOfGuests > updatedTotalCapacity)
                Alert($"Warning: booking saved with {updated.NumberOfGuests} guests but combined table capacity is only {updatedTotalCapacity}. Capacity limit exceeded \u2014 approved override.", AlertType.warning);
            else
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
    [Authorize(Roles = "admin,owner,manager")]
    public IActionResult DeleteConfirm(int id)
    {
        var booking = svc.GetBookingById(id);
        if (booking is null)
        {
            Alert("Booking not found.", AlertType.warning);
            return RedirectToAction(nameof(Index));
        }

        // Block delete if guests are currently seated
        if (booking.Status == "Seated")
        {
            Alert("Cannot delete \u2014 guests are currently seated. Close the booking when they leave.", AlertType.warning);
            return RedirectToAction(nameof(Index));
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

    // POST /Booking/SeatGuests/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin,owner,manager,staff")]
    public IActionResult SeatGuests(int id)
    {
        var booking = svc.GetBookingById(id);
        if (booking is null)
        {
            Alert("Booking not found.", AlertType.warning);
            return RedirectToAction(nameof(Index));
        }

        // Calculate total capacity across all linked tables
        var allTables = svc.GetAllTables().ToList();
        int totalCapacity = 0;
        var primaryTable = allTables.FirstOrDefault(t => t.TableNumber == booking.TableNumber);
        if (primaryTable != null) totalCapacity += primaryTable.SeatingCapacity;
        foreach (var part in (booking.AdditionalTableNumbers ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries))
            if (int.TryParse(part.Trim(), out var tn))
            {
                var extra = allTables.FirstOrDefault(t => t.TableNumber == tn);
                if (extra != null) totalCapacity += extra.SeatingCapacity;
            }

        // Block seating if any of the assigned tables are already occupied
        var occupiedTableNumbers = new List<int>();
        if (primaryTable != null && primaryTable.IsOccupied)
            occupiedTableNumbers.Add(primaryTable.TableNumber);
        foreach (var part in (booking.AdditionalTableNumbers ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries))
            if (int.TryParse(part.Trim(), out var tn))
            {
                var extra = allTables.FirstOrDefault(t => t.TableNumber == tn);
                if (extra != null && extra.IsOccupied)
                    occupiedTableNumbers.Add(extra.TableNumber);
            }

        if (occupiedTableNumbers.Any())
        {
            var tableList = string.Join(", ", occupiedTableNumbers.Select(n => $"T{n}"));
            Alert($"Table {tableList} is already occupied. Please assign a different table to this booking before seating these guests.", AlertType.warning);
            return RedirectToAction(nameof(Edit), new { id });
        }

        bool capacityExceeded = booking.NumberOfGuests > totalCapacity;
        bool canOverride = User.IsInRole("admin") || User.IsInRole("owner") || User.IsInRole("manager");

        if (capacityExceeded && !canOverride)
        {
            Alert($"Cannot seat {booking.NumberOfGuests} guests \u2014 combined table capacity is {totalCapacity}. A manager or owner must approve this override.", AlertType.warning);
            return RedirectToAction(nameof(Index));
        }

        var updated = svc.SeatGuests(id);
        if (updated is not null)
        {
            if (capacityExceeded)
                Alert($"Warning: {updated.NumberOfGuests} guests seated on tables with combined capacity of {totalCapacity}. Capacity limit exceeded \u2014 approved override.", AlertType.warning);
            else
                Alert($"'{updated.CustomerName}' seated. Table {updated.TableNumber} marked occupied.", AlertType.success);
        }
        else
            Alert("Could not seat guests \u2014 booking may already be seated or not found.", AlertType.warning);
        return RedirectToAction(nameof(Index));
    }

    // POST /Booking/CloseBooking/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin,owner,manager,staff")]
    public IActionResult CloseBooking(int id)
    {
        var updated = svc.CloseBooking(id);
        if (updated is not null)
            Alert($"Booking for '{updated.CustomerName}' closed. Table freed.", AlertType.success);
        else
            Alert("Could not close booking \u2014 guests must be seated first.", AlertType.warning);
        return RedirectToAction(nameof(Index));
    }

    // POST /Booking/CancelBooking/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin,owner,manager,staff")]
    public IActionResult CancelBooking(int id)
    {
        var updated = svc.CancelBooking(id);
        if (updated is not null)
            Alert($"Booking for '{updated.CustomerName}' cancelled.", AlertType.success);
        else
            Alert("Could not cancel \u2014 only Booked reservations can be cancelled.", AlertType.warning);
        return RedirectToAction(nameof(Index));
    }

    // GET /Booking/WalkIn — seat walk-in guests immediately
    [HttpGet]
    [Authorize(Roles = "admin,owner,manager,staff")]
    public IActionResult WalkIn(int? tableNumber = null)
    {
        var now = DateTime.Now;
        var horizon = now.AddHours(2);

        // Upcoming reservations within the next 2 hours, keyed by table number
        var reservations = svc.GetAllBookings()
            .Where(b => b.Status == "Booked"
                     && b.BookingDateTime >= now
                     && b.BookingDateTime <= horizon)
            .ToDictionary(b => b.TableNumber, b => b.BookingDateTime);

        ViewBag.AvailableTables = svc.GetAllTables()
            .Where(t => t.Active && !t.IsOccupied)
            .OrderBy(t => t.TableNumber)
            .ToList();

        ViewBag.Reservations = reservations;

        var vm = new BookingViewModel
        {
            TableNumber = tableNumber ?? 0,
            NumberOfGuests = 1,
            BookingDateTime = now,
            CustomerName = "Walk-in"
        };

        return View(vm);
    }

    // POST /Booking/WalkIn
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin,owner,manager,staff")]
    public IActionResult WalkIn(BookingViewModel vm)
    {
        var now = DateTime.Now;
        var horizon = now.AddHours(2);

        var reservations = svc.GetAllBookings()
            .Where(b => b.Status == "Booked"
                     && b.BookingDateTime >= now
                     && b.BookingDateTime <= horizon)
            .ToDictionary(b => b.TableNumber, b => b.BookingDateTime);

        bool tableIsReserved = reservations.ContainsKey(vm.TableNumber);
        bool canOverrideReserved = User.IsInRole("admin") || User.IsInRole("owner") || User.IsInRole("manager");

        if (tableIsReserved && !canOverrideReserved)
        {
            ModelState.AddModelError("TableNumber",
                $"Table {vm.TableNumber} is reserved at {reservations[vm.TableNumber]:HH:mm}. Only a manager or above can seat walk-in guests here.");
        }

        if (vm.TableNumber <= 0)
        {
            ModelState.AddModelError("TableNumber", "Please select a table.");
        }

        if (string.IsNullOrWhiteSpace(vm.CustomerName))
        {
            vm.CustomerName = "Walk-in";
            ModelState.Remove("CustomerName");
        }

        if (!ModelState.IsValid)
        {
            ViewBag.AvailableTables = svc.GetAllTables()
                .Where(t => t.Active && !t.IsOccupied)
                .OrderBy(t => t.TableNumber)
                .ToList();
            ViewBag.Reservations = reservations;
            return View(vm);
        }

        var created = svc.AddBooking(
            vm.CustomerName,
            vm.PhoneNumber ?? "",
            vm.Email ?? "",
            now,
            vm.NumberOfGuests,
            tableNumber: vm.TableNumber,
            isActive: true
        );

        if (created is null)
        {
            Alert("Could not create walk-in booking.", AlertType.warning);
            ViewBag.AvailableTables = svc.GetAllTables()
                .Where(t => t.Active && !t.IsOccupied)
                .OrderBy(t => t.TableNumber)
                .ToList();
            ViewBag.Reservations = reservations;
            return View(vm);
        }

        // Immediately seat — transitions Booked → Seated and marks table occupied
        var seated = svc.SeatGuests(created.Id);
        if (seated is not null)
        {
            if (tableIsReserved)
                Alert($"Walk-in seated at T{vm.TableNumber}. Note: this table has a reservation at {reservations[vm.TableNumber]:HH:mm}.", AlertType.warning);
            else
                Alert($"Walk-in guests seated at T{vm.TableNumber}.", AlertType.success);
        }
        else
        {
            Alert("Booking created but could not be seated automatically. Use the Bookings list to seat manually.", AlertType.warning);
        }

        return RedirectToAction(nameof(Index), "Table");
    }
}
