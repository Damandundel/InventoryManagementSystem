# Inventory Management System — Code Study Guide

A complete walkthrough of **how the program is built and why**, so you can explain
any part of it and answer questions confidently. Read top-to-bottom once; use the
**Q&A** section at the end to rehearse.

---

## 1. The big picture

It's an **ASP.NET Core 8 MVC** web application for managing a warehouse inventory:
products, the categories/suppliers/warehouses they belong to, and the stock
movements (add/remove) that change quantities. On top of that sits a dashboard
with statistics.

Two ideas define the design:

1. **Layered (n-tier) architecture** — the code is split into separate projects so
   each has one job: web UI, business logic, data access, and tests.
2. **Multi-tenancy** — every account only sees and edits *its own* data. The
   example `admin@inventory.com` account is pre-loaded with demo data; a newly
   registered user starts empty and builds their own inventory.

---

## 2. Solution structure (4 projects)

```
InventoryManagementSystem.sln
├── InventoryManagementSystem.Web      → ASP.NET Core MVC app (UI, controllers, the entry point)
├── InventoryManagementSystem.Services → business logic (interfaces + implementations)
├── InventoryManagementSystem.Data     → EF Core: entity models, DbContext, migrations, seeding
└── InventoryManagementSystem.Tests    → NUnit unit tests for the service layer
```

**Dependency direction (very important):**
`Web → Services → Data`. The Web project references Services and Data; Services
references Data; Data references nothing of ours. `Tests` references Services + Data.
Dependencies always point *inward* toward the data layer — the UI never talks to
the database directly, it always goes through a service.

Target framework: **.NET 8** (`net8.0`). EF Core packages: **8.0.8**.

---

## 3. The data layer (`InventoryManagementSystem.Data`)

### 3.1 Entities (the database tables)

| Entity | Key fields | Notes |
|--------|-----------|-------|
| `Product` | Name, Description, **Price** `[Precision(18,2)]`, Quantity, CategoryId, SupplierId, WarehouseId, **OwnerId**, IsDeleted | central entity; has a collection of `StockTransactions` |
| `Category` | Name, Description, **OwnerId**, IsDeleted | has many Products |
| `Supplier` | Name, Email `[EmailAddress]`, PhoneNumber `[Phone]`, **OwnerId**, IsDeleted | the "business" |
| `Warehouse` | Name, Location, **OwnerId**, IsDeleted | has many Products |
| `StockTransaction` | ProductId, QuantityChanged, Type ("Add"/"Remove"), CreatedOn, Note | the audit log of stock changes |
| `ApplicationUser : IdentityUser` | FullName | the logged-in user (extends Identity's built-in user) |

- **Data annotations** drive both the DB schema and validation: `[Required]`,
  `[MaxLength]`, `[EmailAddress]`, `[Phone]`, `[Precision(18,2)]`, `[Key]`,
  `[ForeignKey]`.
- **`OwnerId`** is the multi-tenancy key — it holds the id of the `ApplicationUser`
  who created the row.
- **`IsDeleted`** powers *soft delete* (see §9).

### 3.2 `InventoryDbContext`

Inherits **`IdentityDbContext<ApplicationUser>`**, so it provides both our tables
(`DbSet<Product>`, etc.) **and** all the ASP.NET Identity tables (AspNetUsers,
AspNetRoles, …) in one database.

`OnModelCreating` configures relationships with the **Fluent API**:

- Product → Category / Supplier / Warehouse, and StockTransaction → Product, all
  use **`DeleteBehavior.Restrict`** (you can't delete a parent that still has
  children — prevents accidental cascade wipes).
- Each owned entity gets an `OwnerId` foreign key to `ApplicationUser`
  (`HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.OwnerId)`), also
  Restrict. EF automatically indexes foreign keys, so owner-filtered queries are fast.

### 3.3 Migrations & seeding

- **EF Core Migrations** are code that creates/updates the schema. The
  `Migrations/` folder holds the generated `InitialCreate` migration + a model
  snapshot. They are generated with `dotnet ef migrations add` and applied with
  `Database.MigrateAsync()` (called at startup — see §4).
- **`DbInitializer.SeedDemoDataAsync(context, ownerId)`** creates the demo catalog
  (3 categories, 3 suppliers, 3 warehouses, 5 products) **owned by the admin**. It
  runs at startup and only seeds once (it checks `if (await data.Products.AnyAsync(p => p.OwnerId == ownerId)) return;`).
  Seeding is done *in code at runtime* (not via migration `HasData`) because the
  admin's user id is generated at runtime.

---

## 4. Application start-up & pipeline (`Web/Program.cs`)

This is the composition root — everything is wired here.

**Service registration (DI container):**
- `AddDbContext<InventoryDbContext>(options => options.UseSqlite(connectionString))`
- `AddDefaultIdentity<ApplicationUser>(...).AddRoles<IdentityRole>().AddEntityFrameworkStores<InventoryDbContext>()`
  — sets password rules and plugs Identity into our DbContext.
- `ConfigureApplicationCookie(...)` — sets login/logout/access-denied paths.
- Each service is registered **scoped**: `AddScoped<IProductService, ProductService>()`
  (and the same for category/supplier/warehouse/stock/home). "Scoped" = one
  instance per HTTP request, which matches the DbContext lifetime.
- `AddControllersWithViews()` — enables MVC.

**Middleware pipeline (order matters):** dev exception page (or `UseExceptionHandler("/Error/500")` + HSTS in prod) → `UseStatusCodePagesWithRedirects("/Error/{0}")` → HTTPS redirect → static files → routing → **authentication → authorization** → endpoint routing (`MapControllerRoute` default `{controller=Home}/{action=Index}/{id?}`).

**Startup data work (in a DI scope):**
1. `dbContext.Database.MigrateAsync()` — apply pending migrations (creates the SQLite file the first time).
2. Ensure the `Administrator` role exists.
3. Ensure the `admin@inventory.com` user exists (create + add to role).
4. `DbInitializer.SeedDemoDataAsync(dbContext, adminUser.Id)` — give admin the demo catalog.

This is why **no manual database setup is needed** — first run builds and seeds everything.

---

## 5. The services layer (`InventoryManagementSystem.Services`)

Every entity has an **interface** (`IProductService`) and an **implementation**
(`ProductService`). Controllers depend on the *interface*, never the concrete class
— that's the Dependency Inversion Principle and what makes the code testable.

**Conventions used in every service:**
- Constructor takes `InventoryDbContext` (injected).
- **Every method takes `string ownerId`** and filters by it, so a service can only
  ever touch the caller's data. This is the heart of the multi-tenancy *and* the
  security model.
- **Reads** use `.AsNoTracking()` (no change-tracking overhead) and a **LINQ
  projection** straight into a *service model* (DTO) — e.g.
  `.Select(p => new ProductServiceModel { ... CategoryName = p.Category.Name ... })`.
  Entities never leave the service layer.
- **Writes** load the entity *scoped to the owner*
  (`FirstOrDefaultAsync(p => p.Id == id && p.OwnerId == ownerId)`); if it's null
  the method returns `false`/`null` and the controller shows "not found". This is
  what blocks one user from editing another's records (IDOR-safe).
- All methods are **async** (`await ...Async`) so threads aren't blocked on the DB.

**`ProductService`** factors its repeated projection into one static
`Expression<Func<Product, ProductServiceModel>>` so `GetAll`, `GetById`, `Search`,
and the `GetByCategory/Supplier/Warehouse` filters all reuse it.

**`StockTransactionService.AddStockAsync / RemoveStockAsync`** do two things in one
unit of work: update `Product.Quantity` **and** insert a `StockTransaction` log row,
then `SaveChangesAsync()`. Remove validates there's enough stock first
(`if (product.Quantity < quantity) return false;`).

**`HomeService`** builds the dashboard and statistics. Because **SQLite can't
`SUM`/`ORDER BY` decimals in SQL**, all money math (inventory value, value by
category/warehouse, top products) is done **in memory**: it pulls the
price/quantity pairs with `ToListAsync()` first, then aggregates with LINQ-to-Objects.

**Service models** (`Models/*ServiceModel.cs`) are flat DTOs that carry exactly
what a view needs (e.g. a product's `CategoryName` string, not the whole `Category`).

---

## 6. The web layer (`InventoryManagementSystem.Web`)

### 6.1 Controllers

- **`BaseController`** — abstract, marked **`[Authorize]`** (so every page that
  inherits it requires login) and exposes
  `protected string OwnerId => User.FindFirstValue(ClaimTypes.NameIdentifier)`.
  All data controllers inherit it, so the current user's id is one property away
  and authentication is enforced in one place.
- **`ProductsController`, `CategoriesController`, `SuppliersController`,
  `WarehousesController`** — the classic MVC action set: `Index` (list),
  `Details`, `Create` (GET form + POST), `Edit` (GET + POST), `Delete` (GET confirm
  + `DeleteConfirmed` POST). Each action calls the matching service method, passing
  `OwnerId`.
- **`StockTransactionsController`** — `Index` (history) and `Create` (with an
  optional `productId` to pre-select a product when you click "Adjust Stock").
- **`HomeController`** — `Index` (dashboard) and `Statistics`.
- **`AccountController`** — `Login`, `Register`, `Logout` using Identity's
  `SignInManager`/`UserManager`. Register auto-signs-in and redirects home.
- **`ErrorController`** — renders the custom 400/401/404/500 pages.
- **`Api/ProductsApiController`** — a REST-style `[ApiController]` used by the
  dashboard's live search; also `[Authorize]` and owner-scoped.

**Patterns in the controllers:**
- **Post-Redirect-Get (PRG):** successful POSTs `RedirectToAction` instead of
  returning a view, so refreshing the page won't resubmit the form.
- **`[ValidateAntiForgeryToken]`** on every POST — CSRF protection.
- **`ModelState.IsValid`** gate at the top of POST actions for server-side validation.
- **`TempData["Success"]` / `TempData["Error"]`** — one-time flash messages shown
  after the redirect.

### 6.2 View models (`ViewModels/`)

The web layer has its own models, separate from entities and service models:
- **Form view models** (`ProductFormViewModel`, etc.) carry the editable fields plus
  `DataAnnotations` for validation (`[Required]`, `[Range]`, `[StringLength]`,
  `[Display]`) and the dropdown lists for `<select>`s.
- **Details view models** (`ProductDetailsViewModel`, `CategoryDetailsViewModel`, …)
  hold an entity plus computed extras (e.g. `StockValue => Price * Quantity`,
  aggregate totals, the related products list, and stock history).
- **Dropdown view models** (`CategoryDropdownViewModel`, …) are tiny `{Id, Name}`
  pairs for `<select>` lists.
- **`DashboardViewModel` / `StatisticsViewModel`** aggregate everything the home
  pages render.

### 6.3 Views (Razor) & front-end

- **`_Layout.cshtml`** is the shared shell (navbar, footer, CSS/JS includes,
  `@RenderBody()`).
- **`_ViewImports.cshtml`** declares common `@using` namespaces and
  `@addTagHelper *` so tag helpers work everywhere; **`_ViewStart.cshtml`** sets the
  layout for all views.
- **Partial views:** `_LoginPartial` (login/logout nav), `_TempDataMessages`
  (renders the flash alerts), `_ValidationScriptsPartial` (jQuery validation).
- **Tag helpers** do the heavy lifting in forms and links: `asp-for` (binds an
  input to a model property + emits validation attributes), `asp-action` /
  `asp-controller` / `asp-route-id` (generate URLs), `asp-validation-for` (per-field
  error), `asp-items` (populate a `<select>`), and the automatic hidden
  **anti-forgery token** inside every `<form>`.
- **UX touches:** Bootstrap 5 + a custom dark theme in `wwwroot/css/site.css`;
  `wwwroot/js/site.js` makes whole table rows clickable and auto-dismisses alerts;
  the dashboard uses **jQuery AJAX** to call the API for live product search.

---

## 7. Authentication & authorization

- **Authentication** (who you are): ASP.NET Core **Identity** with cookie auth.
  `AccountController` uses `UserManager`/`SignInManager`; passwords are salted &
  hashed by Identity (never stored in plain text).
- **Authorization** (what you can do): `[Authorize]` on `BaseController` forces
  login for the whole app (`/` redirects to `/Account/Login` when signed out). A
  seeded `Administrator` role exists, but feature access no longer depends on it —
  every authenticated user has full CRUD over **their own** data.
- **Data-level authorization (the important bit):** even if a user crafts a URL
  like `/Products/Edit/1` for someone else's product, the service query filters by
  `OwnerId`, finds nothing, and the controller returns "not found." There's no way
  to reach another tenant's data.

---

## 8. Why SQLite (and the decimal caveat)

- The app uses **SQLite** — a single file (`InventoryManagementSystem.db`) created
  on first run. No server to install, no LocalDB/SQL Server dependency. (It was
  switched from SQL Server LocalDB because LocalDB's runtime kept failing to start.)
- Switching providers was clean precisely because of the layered design — only
  `Program.cs` (one line: `UseSqlite`) and the package reference changed; controllers
  and views were untouched.
- **Caveat:** SQLite stores `decimal` as text and can't aggregate/sort it in SQL,
  so `HomeService` does money totals **in memory** (§5). Values stay exact (the
  dashboard total is `$143,902.50`).

---

## 9. Cross-cutting techniques (quick reference)

| Technique | Where / how |
|-----------|-------------|
| **Dependency Injection** | services registered in `Program.cs`, injected via constructors |
| **Async/await** | every DB call is `...Async` + `await` |
| **LINQ + projections** | EF queries shaped directly into DTOs with `.Select(...)` |
| **Repository-less EF** | the `DbContext` *is* the repository; services are the business layer |
| **Soft delete** | set `IsDeleted = true`; every query filters `!IsDeleted` (data is never lost) |
| **Validation** | server-side via DataAnnotations + `ModelState`; client-side via jQuery unobtrusive validation |
| **PRG pattern** | POST → `RedirectToAction` to avoid double-submit |
| **TempData flash messages** | success/error banners after redirects |
| **Anti-forgery (CSRF)** | `[ValidateAntiForgeryToken]` + auto token in forms |
| **Custom error pages** | `UseStatusCodePagesWithRedirects` + `ErrorController` (400/401/404/500) |
| **AJAX** | dashboard live search calls `ProductsApiController` with jQuery |

---

## 10. Testing (`InventoryManagementSystem.Tests`)

- **NUnit** with EF Core's **in-memory database** provider — each test spins up a
  fresh, isolated DB (`UseInMemoryDatabase(Guid.NewGuid())`) so tests don't touch
  the real file and don't interfere with each other.
- **24 tests** cover `ProductService` and `StockTransactionService`: create/edit/
  soft-delete, search, the category/supplier/warehouse filters, stock add/remove
  with the insufficient-stock rule, and **ownership isolation** (a user can't read,
  edit, or delete another owner's product).
- Run with `dotnet test`.

---

## 11. How to run

1. Open the solution in Visual Studio 2022 and press **F5** (or `dotnet run --project InventoryManagementSystem.Web`).
2. The app **auto-migrates and seeds** on first launch — nothing to set up.
3. Log in as **`admin@inventory.com` / `Admin123!`** to see the demo data, or click
   **Register** to start a fresh, empty account and add your own data.
4. Tests: `dotnet test`.

---

## 12. Likely questions & answers

**Q: Why split it into four projects?**
Separation of concerns and testability. The UI (Web) depends on business logic
(Services) which depends on data access (Data). Because controllers depend on
service *interfaces*, the Tests project can exercise the logic without a web server
or a real database.

**Q: How does multi-tenancy work?**
Every catalog row stores an `OwnerId` (the user's id). Every service method takes an
`ownerId` and filters by it on reads *and* writes. Controllers get the id from
`BaseController.OwnerId` (`User.FindFirstValue(ClaimTypes.NameIdentifier)`). So each
account only ever sees and edits its own data.

**Q: How do you stop one user editing another's data?**
The write methods load the entity with `... && p.OwnerId == ownerId`. If it's not
the caller's, the query returns null and the action reports "not found" — there's no
code path that mutates another tenant's row.

**Q: What's the difference between an entity, a service model, and a view model?**
The **entity** maps to a DB table (Data layer). The **service model** is a flat DTO
returned by services (no EF tracking, only the fields needed). The **view model** is
shaped for a specific page and carries validation annotations and dropdown data.
Keeping them separate stops EF entities leaking into views.

**Q: Why soft delete instead of actually deleting?**
To preserve history and referential integrity (a deleted product still has stock
transactions). We set `IsDeleted = true` and filter it out everywhere.

**Q: How is the database created?**
EF Core migrations describe the schema; `Database.MigrateAsync()` runs at startup
and creates/updates the SQLite file automatically, then the admin + demo data are seeded.

**Q: Where does validation happen?**
Both sides. Server-side: DataAnnotations on the view models checked via
`ModelState.IsValid`. Client-side: jQuery unobtrusive validation reads the same
annotations and validates in the browser before posting.

**Q: What is dependency injection and where is it used?**
The framework creates and supplies objects instead of you `new`-ing them. Services
are registered in `Program.cs` and the DbContext + services are injected into
controllers via their constructors.

**Q: Why is money summed in C# and not SQL?**
SQLite has no real decimal type (it stores decimals as text) and can't aggregate or
order them reliably in SQL, so `HomeService` materializes the rows and sums them in
memory to keep the values exact.

**Q: What does `[ValidateAntiForgeryToken]` do?**
It protects POST actions from cross-site request forgery by requiring the hidden
anti-forgery token that the form tag helper emits — a forged cross-site form won't
have a valid token.
