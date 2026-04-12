# Viva Preparation — Restaurant Management System

These are the questions a lecturer is likely to ask you. Read each one, cover the answer, and try to say it out loud in your own words before reading the answer. If you can explain it out loud, you can explain it at the viva.

---

## 1. Project Architecture

**Q: What is the MVC pattern and where do you see it in your project?**

MVC stands for Model, View, Controller. It separates an application into three parts:
- **Model** — the data and business logic. In your project that is `RMS.Data` — the entities and services.
- **View** — the HTML that gets shown to the user. In your project that is everything in `RMS.Web/Views/`.
- **Controller** — receives HTTP requests, asks the service for data, and decides which view to return. In your project those are in `RMS.Web/Controllers/`.

The point is that each layer only knows about the layer next to it. A view does not talk to the database. A controller does not write SQL.

---

**Q: Why is the project split into two separate C# projects — RMS.Data and RMS.Web?**

So the data layer has no dependency on the web layer. `RMS.Data` knows nothing about HTTP, cookies, or Razor. This means you could swap the web layer out for an API or a mobile app and the data layer would not need to change. It also makes the service layer easier to test in isolation — `RMS.Test` only references `RMS.Data`, not `RMS.Web`.

---

**Q: What is a service layer and why do you use one?**

The service layer sits between the controller and the database. In your project it is `RestaurantServiceDb`, which implements the `IRestaurantService` interface. The controller calls methods like `svc.GetAllBookings()` — it does not query the database directly.

The benefit is that the controller stays thin. It only handles HTTP concerns (what request came in, what response to send back). All the business logic and data access lives in the service.

---

**Q: What is an interface and why does IRestaurantService exist?**

An interface is a contract — it says "any class that implements me must have these methods". `IRestaurantService` lists all the operations a restaurant service must support: `AddBooking`, `GetAllOrders`, `AddTable`, etc.

The controller only depends on `IRestaurantService`, not on `RestaurantServiceDb` directly. This means you could have a `RestaurantServiceFake` (backed by lists in memory) for testing without touching the database — which is exactly what `RMS.Test` does.

---

**Q: What database does this application use?**

SQLite. It is a file-based database — in development the database is a single file at `RMS.Web/RMS.db`. There is no separate database server running. Entity Framework Core manages the schema through the `DatabaseContext`. Every time the app starts in development mode, `ServiceSeeder.Seed()` is called from `Program.cs` to reset and reload test data.

---

## 2. Authentication and Authorisation

**Q: How does login work in your application?**

The user submits their email and password. The `UserController.Login` POST action calls `_svc.Authenticate(email, password)`. If a user is found, `AuthBuilder.BuildClaimsPrincipal(user)` creates a claims principal — a set of key-value pairs that describe the user (their ID, name, email, and role). That principal is stored in an encrypted cookie using `HttpContext.SignInAsync`. On every subsequent request, ASP.NET Core reads that cookie and knows who the user is.

---

**Q: What is a claim?**

A claim is a piece of information about the user that gets stored in their authentication cookie. In your project, four claims are stored:
- `ClaimTypes.Sid` — the user's database ID
- `ClaimTypes.Email` — their email address
- `ClaimTypes.Name` — their display name
- `ClaimTypes.Role` — their role (e.g. "manager")

You can read these back in a controller using `User.FindFirstValue(ClaimTypes.Email)`, or check a role using `User.IsInRole("manager")`.

---

**Q: What roles exist in your system and what are the differences?**

The `Role` enum in `User.cs` defines: `admin`, `owner`, `manager`, `staff`, `autenticated` (legacy typo — kept for compatibility), `guest`.

In practice:
- `admin` and `owner` can void and delete orders. They have full access.
- `manager` and `staff` can create orders, manage bookings, tables, and menus, but cannot void or delete.
- `guest` is a registered customer. They can make bookings and view menus but cannot manage anything.
- Anonymous (not logged in) users can view menus and make bookings — same as guest.

The `[Authorize(Roles = "admin,owner,manager,staff")]` attribute on a controller action means the user must have at least one of those roles. If they do not, they are redirected to the login page.

---

**Q: Why does `autenticated` (with a typo) still exist in the code?**

It was the original role name in the project and had a typo from the start. Early in development, all staff actions used `[Authorize(Roles = "admin,autenticated")]`. The problem was that nobody could ever get that role assigned to them with the right spelling, so staff were constantly redirected to the login page even when logged in. We kept the enum value to avoid breaking any remaining references to it, but no new user is assigned that role.

---

**Q: What is the [ValidateAntiForgeryToken] attribute and why is it on every POST?**

It protects against Cross-Site Request Forgery (CSRF). When ASP.NET Core renders a form, it includes a hidden field with a unique token. When the form is submitted, the server checks that the token matches what it issued. This prevents a malicious website from tricking a logged-in user into submitting a form to your application without their knowledge.

---

## 3. Controllers

**Q: What does BaseController do and why do your controllers inherit from it?**

`BaseController` extends ASP.NET Core's `Controller` class and adds two shared utilities:
1. `Alert(message, type)` — stores a message in `TempData` so it can be displayed on the next page as a Bootstrap alert.
2. `GetSignedInUserId()` — reads the `ClaimTypes.Sid` claim and returns the user's integer ID.

Because `BookingController`, `OrderController`, etc. all inherit from `BaseController`, they all get these methods for free without duplicating the code.

---

**Q: What is TempData and why do you use it for alerts?**

`TempData` is a dictionary that persists for exactly one redirect. You write a message into it, redirect to another action, that action renders a view, the view reads the message and shows it, and then it is gone. It is the right tool for feedback messages after a form submission — you want to show "Booking created successfully" after redirecting to the Index, not before.

---

**Q: What does HTMX do in your project? Where do you see it in the controllers?**

HTMX is a JavaScript library that lets you make partial page updates without a full page reload. When HTMX makes a request, it adds an `HX-Request` header. Your controllers check for this header:

```csharp
if (Request.Headers.ContainsKey("HX-Request"))
{
    return PartialView("Menus", search);
}
return View("Menus", search);
```

If it is an HTMX request, you return only the partial HTML fragment. If it is a normal browser request, you return the full page. This gives you fast search/filter updates without reloading the whole layout.

---

## 4. Bookings

**Q: Walk me through what happens when a customer makes a booking.**

1. The customer visits `/Booking/CreateBooking`. The GET action calls `FindNextAvailableSlot()` to pre-fill the form with the next available time slot. If the user passed a date/time from the home page, it checks from that point. If not, it starts from the next 15-minute boundary.
2. The customer fills in their name, phone, email, number of guests, and optionally comments. They select a date and time.
3. They submit the form. The POST action parses the separate date and time inputs into a `DateTime`, then runs the same 135-minute conflict check. If a conflict is found on every suitable table, it redirects back with a message.
4. If the slot is clear, `svc.AddBooking(vm.ToBooking())` is called and the booking is saved.

---

**Q: How does the time-slot conflict check work?**

Every booking is assumed to occupy 135 minutes (2 hours 15 minutes). When checking whether a new booking at time `candidate` is possible:
1. Get all non-cancelled bookings on the same calendar date.
2. Get all active tables with enough seating capacity for the party size.
3. For each suitable table, check whether any existing booking on that table overlaps with the window `[candidate, candidate + 135 min)`. Two time windows overlap if `candidate < existingEnd AND existing.Start < candidateEnd`.
4. If at least one table has no conflict, the slot is available.

`FindNextAvailableSlot` loops in 15-minute increments up to 8 hours ahead to find the first clear slot.

---

**Q: What is the Status field on a Booking and what values can it have?**

`Status` is a string with three possible values: `"Booked"`, `"Active"`, and `"Cancelled"`. It defaults to `"Booked"` when a booking is created. Staff can change it to `"Active"` when the party arrives, and `"Cancelled"` if the booking does not proceed. The Index and Details views show a colour-coded badge: blue for Booked, green for Active, grey for Cancelled.

---

## 5. Orders and Tables

**Q: How does table occupancy work?**

Three service methods manage it:
- `AddOrder(menuItems, tableId)` — after creating the order and assigning the table, it sets `table.IsOccupied = true` and saves.
- `MarkOrderCompleted(orderId)` — after updating the order, it sets `order.Table.IsOccupied = false`.
- `VoidOrder(orderId)` — same: sets `order.Table.IsOccupied = false`.

So a table is occupied from the moment an order is created on it until the order is either completed or voided.

---

**Q: Why can only admin and owner void or delete orders?**

Voiding and deleting are irreversible financial operations. A void means the order is written off. Deleting removes the record entirely. A staff member could make a mistake or act improperly. Restricting it to admin and owner means there is always an accountable person behind the action. In the controller:

```csharp
[Authorize(Roles = "admin,owner")]
public IActionResult VoidConfirm(int id)
```

---

**Q: What happens to the old table when an order is edited to move to a different table?**

The `OrderController.Edit` POST action handles this:
1. Get the current order from the service.
2. If `vm.TableId` is different from `order.Table.Id`, find both tables.
3. Set the old table's `IsOccupied = false`.
4. Set the new table's `IsOccupied = true`.
5. Update the order with the new items and table.

This keeps occupancy consistent even when orders are moved between tables.

---

**Q: What validations exist on creating a table?**

Three:
1. `[Range(1, 50)]` on TableNumber in the ViewModel — you cannot create more than 50 tables.
2. `[Range(2, 20)]` on SeatingCapacity — a table must seat at least 2 people.
3. A duplicate check in the controller: `svc.GetAllTables().Any(t => t.TableNumber == vm.TableNumber)`. If the table number already exists, a model error is added and the form is redisplayed.

The GET Create action also auto-fills the next table number: `Max(existing numbers) + 1`.

---

## 6. Menus and Ingredients

**Q: What is the relationship between Menu, MenuItem, and Ingredient?**

- A `Menu` has many `MenuItems` (e.g. the Dinner Menu contains Beef Wellington, Crème Brûlée, etc.)
- A `MenuItem` has many `Ingredients` (e.g. Beef Wellington contains beef, pastry, mushroom duxelles)
- An `Ingredient` can have `Allergen = true` and an `AllergenInfo` description

This is a hierarchy: Menu → MenuItem → Ingredient. You get to the allergen information by looking at the ingredients on a menu item.

---

**Q: How does the menu search work with HTMX?**

The `Menus` action accepts a `MenuSearchViewModel` which carries the search parameters and results. When the user types in the search box, HTMX sends a GET request with the `HX-Request` header. The controller checks for that header and returns only the `_Search` partial, not the full page layout. This means only the results section of the page updates, without a full reload.

---

## 7. Allergen Consent

**Q: What is the allergen consent feature for?**

If a customer has an allergen, the booking records `HasAllergen = true`. Before the order proceeds, staff must record formal consent — the customer acknowledges which menu items contain their allergen and signs off. `AllergenConsent` stores their name, email, phone, a list of the menu items they consent to, and whether consent was actually given.

This is a food safety compliance requirement. A restaurant must be able to prove they informed the customer.

---

## 8. Testing

**Q: What framework do you use for testing and where are your tests?**

xUnit. The test project is `RMS.Test/RestaurantServiceTests.cs`. Tests use an in-memory SQLite database — `DatabaseContext` is initialised fresh for each test class, so tests are isolated from each other and from the real database.

---

**Q: How many tests do you have and what do they cover?**

65 tests covering:
- AllergenConsent: add, get, update, delete, get by order
- Ingredients: add, get, update, delete, get by name
- Menu: add, get, update, delete, add/remove menu items, search
- Table: add, get, update, delete, occupancy
- Booking: add, get, update, delete, status transitions (Booked → Active → Cancelled)
- Orders: create empty, create with items (cost calculated), get by id, get all, mark completed, void, delete, table occupied on create, table freed on complete, table freed on void

---

**Q: What does this test verify? Walk me through it:**
```csharp
[Fact]
public void AddOrder_WhenTableAssigned_ShouldMarkTableAsOccupied()
```

It verifies that when you create an order on a table, that table's `IsOccupied` property becomes `true`. The test:
1. Creates a table.
2. Calls `svc.AddOrder(emptyList, tableId)`.
3. Calls `svc.GetTableById(table.Id)` to reload the table from the database.
4. Asserts that `table.IsOccupied == true`.

The reason we reload from the database (rather than using the returned order) is to confirm the change actually persisted to the data store, not just to an in-memory object.

---

## 9. Decisions You Made

**Q: Why did you keep the `autenticated` typo in the Role enum instead of fixing it?**

Renaming an enum value is a breaking change. Any existing user in the database who has `autenticated` stored as their role would lose their role on next login. It is also possible it appears in view-side `IsInRole` checks that we have not caught. The safest decision was to add the correct roles alongside it and use those going forward. In a real production system you would write a database migration to update existing records.

---

**Q: Why do you seed the database every time the app starts in development?**

So every developer starts from a known, consistent state. If the database gets into a bad state during testing, you just restart the app and it is clean again. In production (`Program.cs` only calls `ServiceSeeder.Seed()` inside the `if (app.Environment.IsDevelopment())` block) this does not happen — you would never wipe a production database on startup.

---

**Q: What would you do differently if you had more time?**

Honest answers that show engineering maturity:
- Add a proper database migration system instead of seeding from scratch each time
- Move `new RestaurantServiceDb()` out of the controller constructors and use dependency injection (`services.AddScoped<IRestaurantService, RestaurantServiceDb>()`) — this is the production pattern
- Add integration tests that test the full HTTP request/response cycle, not just the service layer
- Fix the `autenticated` typo properly with a data migration
- Add proper course tracking for orders (starters served, mains ready, dessert)

---

## 10. Things That Tripped You Up (Worth Being Honest About)

These are the bugs that were found during testing. If asked "did you encounter any problems?", these are honest, specific answers:

1. **auth typo** — `[Authorize(Roles = "admin,autenticated")]` silently bounced all staff to the login page. The typo existed from the start and was not caught until full testing. Fixed by expanding all role lists to `"admin,owner,manager,staff"`.

2. **Table occupancy not managed** — `AddOrder` was saving the order but never setting `table.IsOccupied = true`. The table always showed as free even with an active order. Fixed by adding three lines to `AddOrder`, `MarkOrderCompleted`, and `VoidOrder` in `RestaurantServiceDb`.

3. **Booking used table state not time slots** — the original booking conflict check looked at `table.IsOccupied`. That only tells you the table has an active order right now, not whether it has a booking in an hour. Replaced with a 135-minute overlap check against all existing bookings on the same day.

4. **Missing using directive** — `BookingController.cs` used `List<Booking>` and `List<Table>` inside `FindNextAvailableSlot` but was missing `using RMS.Data.Entities;`. The build failed until that line was added. Lesson: always check your `using` directives when adding new code to an existing file.

---

## 11. Sprint Reflection — What Did You Learn and What Would You Do Differently?

This section is specifically for the viva question: "What did you learn during development, and what would you change if you started again?"

---

**Q: What is the most important thing you learned during this sprint?**

That silent failures are the hardest bugs to find. The `[Authorize(Roles = "admin,autenticated")]` typo did not throw an exception. The app still ran. ASP.NET Core just redirected anyone with the wrong role to the login page without any error message. I had no idea it was broken until I actually logged in as a staff user and tested the feature. The lesson is: write a test or manually test every feature as you build it — do not build five things and then test them all at once at the end.

---

**Q: What would you do differently with the role system if you started again?**

I would define the roles correctly from day one and not let a typo survive into a working codebase. The original `autenticated` role was a typo in the initial setup and it stayed there because nobody caught it early. In a real project, the first thing I would do after defining roles is write a test that logs in as each role and checks they can access what they are supposed to. That test would have caught the typo on day one.

---

**Q: What would you do differently with the booking system?**

The original booking conflict check looked at `table.IsOccupied` — that just checks whether the table has an active order right now. It tells you nothing about future bookings. I should have thought about the time dimension from the start: a booking is a reservation for a future window of time, so the conflict check has to compare time windows, not current state. If I started again I would design the `AddBooking` method with a time-slot overlap check built in from the beginning rather than adding it later.

---

**Q: What would you do differently with table occupancy?**

Table occupancy was not managed at all — `AddOrder` saved the order but never set `table.IsOccupied = true`. The table always showed as free even when an order was active on it. This happened because the logic that should have been in the service layer was never written. If I started again I would write the unit tests for `AddOrder`, `MarkOrderCompleted`, and `VoidOrder` first (test-driven development), and those tests would have failed immediately and told me the occupancy tracking was missing.

---

**Q: What is dependency injection and why would you use it instead of `new RestaurantServiceDb()` in the controller constructor?**

Right now every controller creates its own instance of the service directly:

```csharp
public BookingController()
{
    svc = new RestaurantServiceDb();
}
```

This is tightly coupled — the controller is responsible for creating the service and it is hardwired to `RestaurantServiceDb`. If I wanted to swap in a test double or a different implementation I would have to change the controller code.

With dependency injection (DI), `Program.cs` registers the service:

```csharp
builder.Services.AddScoped<IRestaurantService, RestaurantServiceDb>();
```

And the controller just asks for it:

```csharp
public BookingController(IRestaurantService svc)
{
    this.svc = svc;
}
```

ASP.NET Core handles wiring it up. Now in tests you can pass a fake service without touching the controller at all. It is the standard production pattern and the reason `IRestaurantService` exists.

---

**Q: Why did you not use dependency injection in this project if you know it is better?**

Two reasons. First, the university project was already started with `new RestaurantServiceDb()` in every controller and the brief did not ask us to refactor the architecture — it asked us to implement the features. Changing the constructor pattern across every controller would have been a significant refactor with risk of introducing bugs at a critical time. Second, the pattern used is still valid — the interface is still there, the service layer is still separate, and the code works correctly. It is a trade-off between ideal engineering and delivery to a deadline.

---

**Q: What did you learn about git from this project?**

Three practical things:
1. Commit messages matter. `feat: role system, drinks menu, order edit, booking time-slots, table fixes, 65 tests` tells the next person exactly what changed and why. A message like `update` or `changes` is useless when you are trying to find when a bug was introduced.
2. Feature branches exist for a reason. Working on `sprint/booking-fixes-and-security` meant that `main` was always in a working state. If something went badly wrong mid-sprint, main was unaffected.
3. `git add -p` makes you review what you are about to commit line by line. That review step catches debug code, commented-out blocks, and hardcoded values before they end up in the repository.

---

*Last updated: Manual testing session, 12 April 2026. 77 tests passing, 0 build errors.*

---

## Bugs Found During Manual Testing (12 April 2026)

**Q: How did you find bugs in your application?**

Manual browser testing across all six roles: admin, owner, manager, staff, guest, and anonymous. For each role I tested every page and every action — creating, editing, deleting, searching, and navigating. One full pass took roughly an hour. Nine bugs were found and fixed in the same session.

---

**Q: One of your delete operations was crashing with a foreign key error. What caused it and how did you fix it?**

The `AllergenConsent` entity has a `MenuItems` collection linked through a join table in the database. When you try to delete a parent row while child rows in the join table still reference it, SQLite throws a foreign key constraint violation — it refuses to leave orphan records.

The fix is to load the full entity including its related collection using `.Include(ac => ac.MenuItems)`, then call `consent.MenuItems.Clear()` before removing the consent itself. Entity Framework Core tracks that cleared collection and generates DELETE statements for the join rows as part of the same `SaveChanges` call. Parent is only deleted after the children are gone.

---

**Q: Why did editing a booking let you assign more guests than the table could seat?**

The Edit POST action had no business logic validation — it only ran model state validation (required fields, data types). The actual rule "a table must have capacity >= number of guests" was never checked server-side. The fix is to add explicit validation in the controller before calling the service: look up the table, compare its `SeatingCapacity` against `vm.NumberOfGuests`, and return the form with an error if the check fails. The table input was also changed from a free text number field to a dropdown of real active tables so staff cannot type a table number that does not exist.

---

**Q: You could search menu items but the menu search did nothing. Why were they different?**

The menu items search was implemented correctly in its controller action — the query was used to filter results. The menus search action called `svc.SearchMenus()` to get all menus and then completely ignored `search.Query`. The form was submitting the right data; the controller just was not reading it. One-line fix: add `.Where(m => m.Name.Contains(search.Query))` after the initial fetch when the query is not empty.

---

**Q: What is the difference between client-side and server-side validation, and which do you use?**

Client-side validation runs in the browser using JavaScript before the form is submitted — it gives instant feedback without a round trip to the server. In this project that is jQuery Unobtrusive Validation, wired up via `_ValidationScriptsPartial`. It reads the `data-val-*` attributes that ASP.NET Core MVC generates from your data annotations (`[Required]`, `[Range]`, `[RegularExpression]` etc.).

Server-side validation runs in the controller after the form POST arrives. `ModelState.IsValid` checks the same annotations again. This is the one that actually matters for security — client-side validation can be bypassed by anyone with browser dev tools. Business rules that go beyond data annotations (like "table capacity must be >= guest count") must also be added server-side manually because there is no annotation for that.

This project uses both.

---

## Bugs Found During Manual Testing — Pass 2 (12 April 2026, Owner Role)

**Q: Why could the owner not see the Create Menu button when admin could?**

The submit button on `Menu/Create.cshtml` used the `asp-condition` tag helper with `User.HasOneOfRoles("admin,authenticated")`. Owner is not in that list. The `asp-condition` tag helper suppresses the element entirely when the condition is false — the button rendered as nothing. Fixed by changing the roles string to `"admin,owner,manager,staff"` which matches what the controller's `[Authorize]` attribute already allowed.

---

**Q: The Drinks menu showed all its items on the Edit page but none on the Details page. How did that happen?**

The `_MenuMenuItems` partial was typed as `@model List<MenuItem>` and detected whether it was a drinks menu by checking if *all* items in the list had drink types (`Model.All(...)`). The Details page was passing `Model.MenuItems` — correct. But the detection logic was fragile: one item with an unexpected type like "Mixer" would make `All()` return false and the partial would fall through to food sections, showing nothing.

The fix was to change the partial's model to `MenuViewModel` so it can read `Model.Type` directly. `"Drinks"` is an explicit string comparison — clean and reliable. The Details view was updated to pass the full `Model` instead of just `Model.MenuItems`.

---

**Q: How did you handle the fact that seasonal menus were seeded with type "Seasonal" but the Edit form only offered Lunch, Dinner, Special, Drinks?**

Simple oversight — the dropdown was never given a "Seasonal" option. Editing a Christmas or Summer BBQ menu would replace the type with blank or something invalid. Added "Seasonal" as an option to the `<select>` in both `Create.cshtml` and `Edit.cshtml`. No data layer change needed.

---

**Q: Why were allergen badge HTML tags showing as literal text in the ingredient search list?**

Razor HTML-encodes the output of `@()` expressions by default as a security measure — this is protection against XSS (Cross-Site Scripting). When you write `@(ing.Allergen ? "<span class='badge'>Allergen</span>" : "")`, Razor escapes the angle brackets to `&lt;span&gt;` so they appear as text rather than rendering as HTML.

The fix was to remove the badge entirely from the label — the user just needs the ingredient name. If you needed the badge to render you would use `@Html.Raw(...)`, but that should only be used with trusted content you control, never with user-supplied data.

---

**Q: How did you block deleting a booking when the customer already has an active order?**

In `BookingController.DeleteConfirm`, before calling `svc.DeleteBooking`, the code now checks `booking.OrderId > 0` and if so fetches the order with `svc.GetOrderById`. If the order exists and is neither completed nor voided, the delete is blocked and the user gets a warning message. The booking is only deleted if there is no order, or the order is already finished.

---

**Q: How did you implement table joining for large party bookings?**

Added a single string field `AdditionalTableNumbers` to the `Booking` entity — it stores a comma-separated list of table numbers, e.g. `"3,5"`. Zero configuration, no join table, no EF navigation property changes.

On the Edit view, below the primary table dropdown there is a scrollable checkbox list of all active tables. The form posts the ticked values as `additionalTableNumbers[]`. In the controller Edit POST, the selected IDs are joined back into the comma-separated string and stored on the booking. Capacity validation adds up the seating capacity of the primary table plus every additional table and rejects the save if the total is less than the number of guests.

On the booking list, joined tables display as `T2 + T5 + T8`.

The production-correct approach would be a `BookingTable` join table with a proper many-to-many EF relationship. The string approach was chosen deliberately for scope — it achieves the same result with no added risk to a working system two days before a viva.

---

*Last updated: Owner role testing session, 12 April 2026.*
