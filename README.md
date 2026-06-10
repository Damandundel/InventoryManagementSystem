# Inventory Management System

ASP.NET Core 8 MVC application for tracking products, suppliers, warehouses, categories, and stock movements.

## Features

- ASP.NET Core 8 MVC with Razor Views
- Microsoft SQL Server with Entity Framework Core
- ASP.NET Core Identity with custom `ApplicationUser` (FullName property)
- Administrator role with seeded admin account
- Layered architecture: Web, Data, Services, Tests
- CRUD for Products, Categories, Suppliers, and Warehouses
- Stock add/remove transactions
- Dashboard with statistics and low stock alerts
- API controller (`/api/ProductsApi`)
- AJAX product search on the dashboard
- Server-side and client-side validation
- TempData success/error messages
- Custom error pages (400, 401, 404, 500)
- Soft delete (IsDeleted) instead of permanent deletion
- Bootstrap 5 responsive design
- NUnit unit tests for service layer

## Admin Account

Email: `admin@inventory.com`  
Password: `Admin123!`  
Role: `Administrator`

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
4. Open a terminal in the solution root and run:

```bash
dotnet ef migrations add InitialCreate --project InventoryManagementSystem.Data --startup-project InventoryManagementSystem.Web
dotnet ef database update --project InventoryManagementSystem.Data --startup-project InventoryManagementSystem.Web
```

Or in Package Manager Console:

```powershell
Add-Migration InitialCreate -Project InventoryManagementSystem.Data -StartupProject InventoryManagementSystem.Web
Update-Database -Project InventoryManagementSystem.Data -StartupProject InventoryManagementSystem.Web
```

5. Press F5 to run the application.
6. Log in with the admin account above to access all features.

## Running Tests

```bash
dotnet test
```

## Connection String

Default (LocalDB):
```
Server=(localdb)\MSSQLLocalDB;Database=InventoryManagementSystemDb;Trusted_Connection=True;MultipleActiveResultSets=true
```
