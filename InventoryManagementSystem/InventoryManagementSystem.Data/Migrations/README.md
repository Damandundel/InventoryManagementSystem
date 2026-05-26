# Migrations

Run these commands from the solution folder after opening in Visual Studio or terminal:

```bash
dotnet ef migrations add InitialCreate --project InventoryManagementSystem.Data --startup-project InventoryManagementSystem.Web
dotnet ef database update --project InventoryManagementSystem.Data --startup-project InventoryManagementSystem.Web
```

The DbContext already contains seeded data and an administrator user.
