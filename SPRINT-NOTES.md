# Sprint Notes — Booking Fixes & Security
**Date:** 8 April 2026  
**Branch:** `sprint/booking-fixes-and-security`  
**Sprint Goal:** Fix all booking security holes, broken guest flow, and UX issues so the app works correctly for all user types.

---

## Problems Identified & Solutions

---

### Problem 1 — Guests forced to log in to make a booking
**Observed:** The `/Booking/Create` route had `[Authorize(Roles = "admin,authenticated")]` on both GET and POST actions. A guest visiting the restaurant website could not make a booking without first registering or logging in.  
**Why it matters:** In a real restaurant system, a guest booking is a public-facing feature. Requiring authentication to make a reservation is a UX failure and loses customers.  
**Solution:** Removed the `[Authorize]` attribute from the `Create` GET and POST actions. The booking is now open to anonymous users. `IsActive` is hardcoded to `true` server-side so guests cannot manipulate it.

---

### Problem 2 — Users can self-assign admin role on registration
**Observed:** The `UserRegisterViewModel` exposed a `Role` property with `[Required]`, and the Register form presented a dropdown allowing any registering user to select `admin` or `authenticated`.  
**Why it matters:** This is a critical security vulnerability. Any person visiting the site could register themselves as an admin and gain full access to staff/management functions. This is analogous to a broken access control issue (OWASP Top 10 — A01:2021).  
**Solution:** Removed `Role` from the view model's exposed/bound fields. The `Register` controller action now hardcodes `Role.guest` when calling `_svc.Register(...)`. The `Role` field was also removed from the Register view. Admin accounts can only be created by existing admins or via the seeder.

---

### Problem 3 — IsActive checkbox shown to customers on booking form
**Observed:** The booking Create view presented an `IsActive` checkbox that the customer could check or uncheck. A customer has no concept of whether their booking is "active" — that is a staff management concern.  
**Why it matters:** Exposing internal state flags to end users is poor UX and a minor security concern — a customer could submit a booking with `IsActive = false`, effectively creating a dead booking.  
**Solution:** Removed the `IsActive` field from the Create view entirely. The controller hardcodes `IsActive = true` on booking creation. The Edit view (staff-only) retains the field.

---

### Problem 4 — NumberOfGuests label shows as "NumberOfGuests" (no spaces)
**Observed:** The label on the booking form rendered as `NumberOfGuests` because the property name was used directly without a `[Display(Name = "...")]` annotation.  
**Why it matters:** This is user-facing text. It reads as a programming artefact, not a natural English label. It reduces the perceived quality of the application.  
**Solution:** Added `[Display(Name = "Number of Guests")]` to the `NumberOfGuests` property in `BookingViewModel`.

---

### Problem 5 — Email validation accepts addresses without a valid TLD (e.g. `vbnfn@sf`)
**Observed:** The `[EmailAddress]` data annotation in .NET only checks that there is an `@` symbol and some characters either side. It does not validate that the domain has a top-level domain (e.g. `.com`, `.ie`).  
**Why it matters:** Invalid email addresses would be stored and could never be used to contact the customer (booking confirmations, reminders, etc.).  
**Solution:** Replaced `[EmailAddress]` with a `[RegularExpression]` attribute that requires the format `something@something.tld` where the TLD is at least 2 characters. This covers `.com`, `.ie`, `.co.uk` etc.

---

### Problem 6 — Phone number has no format or length validation
**Observed:** The phone number field accepted any string of any length — including 3-character inputs and 15+ character strings with no structure.  
**Why it matters:** Phone numbers are used to contact customers about their bookings. Garbage data stored here is useless. Also relevant from a data quality and GDPR standpoint.  
**Solution:** Added a `[RegularExpression]` validator that accepts:
- Irish mobile/landline: starting with `08` or `0`, 9-10 digits
- Irish international: `+353` followed by 7-9 digits  
- UK international: `+44` followed by 7-10 digits  
- Northern Ireland local format: `028` followed by 8 digits  
This covers the target audience (Republic of Ireland and Northern Ireland customers).

---

### Problem 7 — Customer name can be a single character
**Observed:** No minimum length was enforced on the `CustomerName` field. A booking could be created with `"A"` as the customer name.  
**Why it matters:** Single-character names are almost certainly erroneous input. A minimum of 2 characters (to allow names like "Li") is a reasonable floor.  
**Solution:** Added `[MinLength(2, ErrorMessage = "Customer name must be at least 2 characters.")]` to `CustomerName` in `BookingViewModel`.

---

### Problem 8 — Booking not saving for logged-in guest user
**Observed:** When a user logged in with the `guest` role attempted to create a booking, it appeared to fail silently — no booking was recorded.  
**Why it matters:** The core booking flow was broken for the primary end-user role.  
**Solution:** The `[Authorize(Roles = "admin,authenticated")]` on the Create actions was blocking guests. Removing the attribute entirely (see Problem 1) fixed this as a side effect. The route is now public and works for anonymous users, guests, and staff alike.

---

### Problem 9 — Booking datetime shows current time, not a 15-minute slot
**Observed:** The `BookingDateTime` field defaulted to `DateTime.Now`, meaning the raw current time (e.g. 17:43:22) was pre-populated. Bookings are not taken at arbitrary times — they should be at standard 15-minute intervals (e.g. 18:00, 18:15, 18:30).  
**Why it matters:** A restaurant operates on a schedule. Having bookings recorded at random seconds past the hour makes the booking list unreadable and the system look unprofessional.  
**Solution:** The controller rounds `DateTime.Now` up to the nearest 15-minute interval when building the initial view model for the Create form. The view uses `step="900"` (900 seconds = 15 minutes) on the `datetime-local` input to enforce 15-minute steps in the browser picker.

---

### Problem 10 — Pre-fill booking details for logged-in users
**Observed:** When a logged-in user navigated to the booking form, their name and email were not pre-populated despite being stored in their account.  
**Why it matters:** Forcing a logged-in user to re-type information the system already knows is poor UX. It also increases the chance of data inconsistency (user books under a slightly different name/email).  
**Solution:** In the `Create` GET action, the controller checks if the user is authenticated (`User.Identity.IsAuthenticated`). If so, it reads the name and email claims from the cookie and pre-populates the view model before passing it to the view.

---

## What We Learned / Concepts Used

- **OWASP A01 — Broken Access Control:** The role self-assignment issue is a textbook example. Never trust the client with privilege decisions.
- **Data Annotations vs. Regex:** .NET's built-in `[EmailAddress]` and `[Phone]` attributes are deliberately lenient. For real-world validation, `[RegularExpression]` gives you precise control.
- **[Authorize] placement matters:** Putting `[Authorize]` on a public-facing action is a design decision, not just a coding task. Think about who the user of each action is.
- **Claims principal:** ASP.NET Core stores the logged-in user's identity as claims in a cookie. You can read `User.FindFirstValue(ClaimTypes.Name)` etc. in a controller action.
- **Display attributes:** `[Display(Name = "...")]` controls what the label tag helper renders. Always use it for user-facing fields.

---

---

### Problem 11 — Home page booking partial did not carry date, time, or guest count to /Booking/Create
**Observed:** The home page has a "Reserve a Table" widget where a customer picks a date, time, and party size then clicks Next. Clicking Next navigated to `/Booking/Create` but all fields were blank — the selected values were lost entirely.  
**Why it matters:** The home page widget is the primary customer entry point. If selections don't carry over, the customer has to enter everything twice — or gives up.  
**Root cause:** The form used `method="post"`. A POST sends data in the request body. The `Create` GET action does not read request bodies — that is what the `[HttpPost]` action is for. The GET action had no parameters at all, so the posted data was simply discarded. On top of this, the field names in the partial (`PartySize`, `BookingDate`, `BookingTime`) did not match the `BookingViewModel` property names, so binding would have failed even if the method was correct.  
**Solution:** Changed the form to `method="get"`. A GET form appends its values to the URL as query string parameters: `/Booking/Create?numberOfGuests=2&bookingDate=2026-04-10&bookingTime=14:00`. The `Create` GET action was updated to accept three optional parameters (`bookingDate`, `bookingTime`, `numberOfGuests`), parse and combine the date and time into a `DateTime`, and pre-populate the view model. If none are provided (direct navigation), it falls back to the next 15-minute boundary as before.

---

### Problem 12 — Home page time dropdown used 30-minute intervals instead of 15
**Observed:** The time selector on the home page booking widget offered times in 30-minute steps (12:00, 12:30, 13:00...). The booking form itself uses 15-minute slots.  
**Why it matters:** Inconsistency between the two entry points. A customer picking 12:15 via the Create form directly was a valid slot, but that option didn't exist on the home widget.  
**Solution:** Changed the loop increment in the `_Booking.cshtml` partial from `TimeSpan.FromMinutes(30)` to `TimeSpan.FromMinutes(15)`.

---

### Problem 13 — Registration email validation still accepted addresses without a TLD
**Observed:** After fixing email validation on the booking form, the same flaw existed on the registration form. `Corey@g` was accepted as a valid email at sign-up.  
**Why it matters:** A user registered with a garbage email address can never receive password reset links, booking confirmations, or other communications. The data is permanently bad.  
**Root cause:** `UserRegisterViewModel` was still using `[EmailAddress]` — the same lenient built-in attribute that was already replaced on `BookingViewModel`.  
**Solution:** Replaced `[EmailAddress]` with the same `[RegularExpression]` requiring an `@`, a domain, and a TLD of at least 2 characters. Applied consistently across both view models.

---

### Problem 14 — Guests could select their own table number on the booking form
**Observed:** The booking Create form included a `TableNumber` input field with a default value of 0. A customer had no idea what table number to enter, table 0 does not exist, and a party of 5 selecting a table for 2 would create a seating conflict.  
**Why it matters (business logic):** A guest choosing their own table removes the restaurant's ability to optimise seating. A party of 2 occupying a table for 8 means the business loses 6 potential covers. This is a direct revenue impact. Guests also should not know their table number in advance — the host seats them on arrival.  
**Why it matters (UX):** Asking a customer to enter a table number they cannot know is confusing and incorrect.  
**Solution:** Removed `TableNumber` from the Create view entirely. In the Create POST action, the controller now queries all tables via the service layer, filters for tables that are `Active`, not `IsOccupied`, and have a `SeatingCapacity >= numberOfGuests`, then sorts by `SeatingCapacity` ascending and takes the first. This ensures the tightest fit — a party of 2 gets a table for 2, not a table for 6. If no table is available it falls back to 0 so staff can handle it manually.

```csharp
var availableTable = svc.GetAllTables()
    .Where(t => t.Active && !t.IsOccupied && t.SeatingCapacity >= vm.NumberOfGuests)
    .OrderBy(t => t.SeatingCapacity)
    .FirstOrDefault();

int assignedTableNumber = availableTable?.TableNumber ?? 0;
```

- `Where` — filters to tables that can physically fit the party and are available
- `OrderBy ascending` — sorts smallest capacity first, ensuring best fit
- `FirstOrDefault` — takes the first result, or `null` if none match (null-safe via `??`)

---

## Sprint Review Notes
*(Fill in at end of sprint)*

- What worked well:
- What would you do differently:
- Outstanding issues for next sprint:

---

---

# Manual Testing Session — 12 April 2026
**Tested as:** admin  
**Build state:** 77 tests passing, 0 failures  
**Branch:** main  

First full browser test of the completed application. Nine bugs found and fixed in the same session.

---

### Bug 1 — Booking/Create was a blank white page
**Observed:** Clicking "New Booking" on `/Booking` produced a blank white page at `/Booking/Create`.  
**Root cause:** `Index.cshtml` had `asp-action="Create"` but the controller GET action is named `CreateBooking`. The router found no matching route and returned an empty response instead of a 404.  
**Fix:** Changed the link in `Booking/Index.cshtml` to `asp-action="CreateBooking"`.

---

### Bug 2 — AllergenConsent delete crashed with SQLite FK error
**Observed:** Deleting an allergen consent record threw `SqliteException: 'FOREIGN KEY constraint failed'`.  
**Root cause:** The `AllergenConsent` entity has a `MenuItems` collection linked through a join table. SQLite enforces foreign key constraints — you cannot delete a parent row while child rows still reference it. The `DeleteAllergenConsent` method was trying to remove the record without first removing the join table entries.  
**Fix:** Load the consent with `.Include(ac => ac.MenuItems)`, call `consent.MenuItems.Clear()` to remove the join entries, then delete the parent. EF Core tracks the cleared collection and removes the join rows in the same `SaveChanges` call.

---

### Bug 3 — Menu search bar returned all menus regardless of query
**Observed:** Searching for "drinks" or "D" on the Menus page returned the full menu list.  
**Root cause:** The `Menus` action in `MenuController` called `svc.SearchMenus()` (which returns all menus) but never applied `search.Query` to filter the result. The search form was posting correctly — the controller just ignored the value.  
**Fix:** After getting all menus, apply a `Where(m => m.Name.Contains(search.Query))` filter when `Query` is not empty.

---

### Bug 4 — MenuItem Type dropdown had no drink options
**Observed:** When adding or editing a menu item, the Type dropdown only offered Starter, Main, Dessert, Side. There was no way to create or edit a drink item — editing an existing drink reset its type to blank.  
**Root cause:** The `<select>` in both `AddMenuItem.cshtml` and `EditMenuItem.cshtml` simply did not have drink options.  
**Fix:** Added Cocktail, Hot Drink, Soft Drink, Beer, Wine as `<option>` values in both views. These are drink subtypes, not a generic "Drink" category, because that gives the kitchen and staff meaningful categorisation.

---

### Bug 5 — Booking edit allowed assigning more guests than a table's capacity
**Observed:** Editing a booking to 3 guests and leaving it on a 2-seat table saved without error. Also, the table number was a free text input — you could type any number including 0 ("Unassigned").  
**Root cause:** The Edit POST action had no validation beyond `ModelState.IsValid`. It called `UpdateBooking` directly without checking the chosen table's capacity or whether the table existed.  
**Fix:** Added server-side validation in the Edit POST: block if `TableNumber <= 0`, block if the table does not exist, block if `table.SeatingCapacity < vm.NumberOfGuests`. Also replaced the free number input in the Edit view with a `<select>` dropdown populated from active tables, showing each table's capacity.

---

### Bug 6 — User management had no Create, Edit (name/email), or Delete
**Observed:** Admin could only change a user's role. Could not fix a wrong email, could not add a new staff account, could not remove a user who had left.  
**Root cause:** These operations were never implemented. `IUserService` only had `UpdateUserRole`.  
**Fix:** Added `UpdateUser(int id, string name, string email)` and `DeleteUser(int id)` to `IUserService` and `UserServiceDb`. Added `Create`, `Edit`, and `Delete` actions to `UserController`. Created three new views (`User/Create.cshtml`, `User/Edit.cshtml`, `User/Delete.cshtml`). Added "Add User" button and Edit/Delete buttons to `User/Index.cshtml`.

---

### Bug 7 — Table edit allowed duplicate table numbers
**Observed:** You could edit Table 1 and change its number to 2 when Table 2 already existed, giving you two Table 2s.  
**Root cause:** The Create POST already had a duplicate check. The Edit POST did not.  
**Fix:** Added the same check to Edit POST, but with `t.Id != id` to exclude the record being edited from the uniqueness check.

---

### Bug 8 — Table with active orders could be deleted
**Observed:** Deleting a table that had an open (non-completed, non-void) order on it left the order with no table, corrupting the data.  
**Root cause:** The Delete POST did no pre-check.  
**Fix:** Before deleting, call `svc.GetOrdersByTableId(id)` and check for orders where `!IsCompleted && !IsVoid`. If any exist, redirect with a warning message instead of deleting.

---

### Bug 9 — Menu items list had no ordering
**Observed:** The `/Menu/MenuItems` page showed items in database insertion order — drinks mixed in with starters and mains.  
**Root cause:** `GetAllMenuItems()` returns items in insertion order. No sort was applied in the controller.  
**Fix:** After filtering, sort by a type priority dictionary: Starter(1) → Main(2) → Dessert(3) → Side(4) → Cocktail(5) → Hot Drink(6) → Soft Drink(7) → Beer(8) → Wine(9), then alphabetically by name within each group.
