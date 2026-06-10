# Inventory Management System

ASP.NET Core 8 MVC application for tracking products, suppliers, warehouses, categories, and stock movements.

> **Database:** uses **SQLite** — a single self-contained file (`InventoryManagementSystem.db`) created automatically on first run. No SQL Server, LocalDB, or any database server installation is required.

## Features

- ASP.NET Core 8 MVC with Razor Views
- SQLite with Entity Framework Core (zero-setup, file-based — no DB server required)
- ASP.NET Core Identity with custom `ApplicationUser` (FullName property)
- **Multi-tenant data ownership** — every account has its own private inventory.
  The example `admin@inventory.com` account ships with a ready-made demo catalog;
  newly registered users start empty and add their own businesses (suppliers),
  warehouses, categories, and products from scratch.
- **Every signed-in user can add / edit / delete** their own products, categories,
  suppliers, and warehouses, and record stock movements. Users can only ever see
  or modify data they own (enforced in the service layer — no cross-account access).
- Automatic database migration and demo-data seeding on startup
- Layered architecture: Web, Data, Services, Tests
- CRUD for Products, Categories, Suppliers, and Warehouses
- Dedicated **detail pages** for Products, Categories, Suppliers, and Warehouses,
  each showing aggregate stats (item count, total quantity, total value) and the
  related products
- Fully cross-linked UI — every entity (category, supplier, warehouse, product)
  is clickable and navigates to its detail page; table rows are clickable too
- Supplier emails/phones are click-to-contact (`mailto:` / `tel:`) and warehouse
  locations link out to Google Maps
- Per-product stock movement history on the product detail page
- "Adjust Stock" / "Restock" shortcuts that pre-select the relevant product
- Stock add/remove transactions
- Dashboard with statistics, low stock alerts, and clickable stat cards
- Statistics page with clickable category/warehouse breakdowns and a top-products leaderboard
- API controller (`/api/ProductsApi`)
- AJAX product search on the dashboard
- Server-side and client-side validation
- TempData success/error messages
- Breadcrumb navigation on detail pages
- Custom error pages (400, 401, 404, 500)
- Soft delete (IsDeleted) instead of permanent deletion
- Bootstrap 5 responsive dark-theme design
- NUnit unit tests for the service layer

## Example Account

A pre-seeded example account comes with a full demo catalog (categories, suppliers,
warehouses, and products) so you can explore the app immediately:

Email: `admin@inventory.com`  
Password: `Admin123!`

To experience the empty-start workflow, **register a new account** instead — it
begins with no data, and you build your inventory by adding your own businesses,
warehouses, categories, and products.

## Project Structure

```
InventoryManagementSystem/
├── InventoryManagementSystem.sln
├── README.md
│
├── InventoryManagementSystem.Data/
│   ├── Constants/
│   ├── Migrations/
│   ├── Models/
│   ├── InventoryDbContext.cs
│   └── InventoryManagementSystem.Data.csproj
│
├── InventoryManagementSystem.Services/
│   ├── Constants/
│   ├── Contracts/
│   ├── Implementations/
│   ├── Models/
│   └── InventoryManagementSystem.Services.csproj
│
├── InventoryManagementSystem.Tests/
│   ├── ProductServiceTests.cs
│   ├── StockTransactionServiceTests.cs
│   └── InventoryManagementSystem.Tests.csproj
│
└── InventoryManagementSystem.Web/
    ├── Controllers/
    │   └── Api/
    ├── ViewModels/
    ├── Views/
    ├── wwwroot/
    ├── Program.cs
    ├── appsettings.json
    └── InventoryManagementSystem.Web.csproj
```

## How to Run

1. Open `InventoryManagementSystem.sln` in Visual Studio 2022.
2. Set `InventoryManagementSystem.Web` as the startup project.
3. Update the connection string in `appsettings.json` if needed (default uses LocalDB).
4. Press F5 (or run `dotnet run --project InventoryManagementSystem.Web`).

   The application **applies any pending EF Core migrations and seeds the admin
   account automatically on startup**, so no manual database setup is required.

5. Log in with the admin account above to access all features.

If you prefer to create/update the database manually (for example, when adding a
new migration), you can still use the EF Core CLI from the solution root:

```bash
dotnet ef migrations add <MigrationName> --project InventoryManagementSystem.Data --startup-project InventoryManagementSystem.Web
dotnet ef database update --project InventoryManagementSystem.Data --startup-project InventoryManagementSystem.Web
```

## Running Tests

```bash
dotnet test
```

## Connection String

Default (SQLite — `appsettings.json`):
```
Data Source=InventoryManagementSystem.db
```

The database file is created next to the running app on first launch and is
ignored by Git. Delete it any time to get a fresh, re-seeded database.
