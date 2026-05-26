# Inventory Management System

ASP.NET Core MVC final exam project built for the Applied Programmer ASP.NET MVC requirements.

## Topic
Inventory Management System for tracking products, suppliers, warehouses, categories and stock movements.

## Main Features

- ASP.NET Core 8 MVC
- Microsoft SQL Server database
- Entity Framework Core with seeded data
- ASP.NET Core Identity with custom `ApplicationUser` properties
- Administrator role
- Separate projects for Web, Data, Services and Tests
- CRUD for Products, Categories, Suppliers and Warehouses
- Stock in/out/adjustment transactions
- Dashboard statistics
- Low-stock page
- API controller for product search and low-stock data
- AJAX product search on the dashboard
- Server-side and client-side validation
- TempData success/error messages
- Custom 400, 401, 404 and 500 error pages
- NUnit tests for business logic

## Admin Account

Email: `admin@inventory.local`  
Password: `Admin123!`

## Project Structure

```text
InventoryManagementSystem.Web       MVC, views, controllers, Identity UI
InventoryManagementSystem.Data      EF Core DbContext and entity models
InventoryManagementSystem.Services  Business logic and service contracts
InventoryManagementSystem.Tests     NUnit tests
```

## How to Run

1. Open `InventoryManagementSystem.sln` in Visual Studio 2022 or newer.
2. Set `InventoryManagementSystem.Web` as startup project.
3. Update the connection string in `InventoryManagementSystem.Web/appsettings.json` if needed.
4. Open Package Manager Console and run:

```powershell
Update-Database -Project InventoryManagementSystem.Data -StartupProject InventoryManagementSystem.Web
```

Or with CLI:

```bash
dotnet ef database update --project InventoryManagementSystem.Data --startup-project InventoryManagementSystem.Web
```

5. Start the project.

## Creating a Fresh Migration

```bash
dotnet ef migrations add InitialCreate --project InventoryManagementSystem.Data --startup-project InventoryManagementSystem.Web
dotnet ef database update --project InventoryManagementSystem.Data --startup-project InventoryManagementSystem.Web
```

## Unit Tests and Coverage

```bash
dotnet test --collect:"XPlat Code Coverage"
```

## Source Control Plan

Mandatory GitHub checklist:

- Public repository from day one
- At least 25 commits
- Commits across at least 7 different days
- README included
- No commits after the final deadline

Suggested commit sequence:

1. Initial solution structure
2. Add data models
3. Add DbContext and seed data
4. Add service contracts
5. Add product service
6. Add CRUD controllers
7. Add product views
8. Add category views
9. Add supplier views
10. Add warehouse views
11. Add stock transaction feature
12. Add dashboard
13. Add AJAX API search
14. Add custom error pages
15. Add Identity role navigation
16. Add validation improvements
17. Add TempData notifications
18. Add admin dashboard
19. Add unit tests
20. Add styling polish

## Defense Talking Points

- Controllers only coordinate HTTP requests and views.
- Business logic is in services.
- EF Core protects against SQL injection through LINQ parameterization.
- Anti-forgery tokens protect POST forms from CSRF.
- Razor encoding helps against XSS.
- Role authorization protects admin-only actions.
- Soft delete avoids accidental data loss.
- API controller is used by AJAX search.
