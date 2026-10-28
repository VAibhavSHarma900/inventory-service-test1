# Inventory Item Management Microservice

A .NET 8 Web API with PostgreSQL for creating, reading, listing, updating, and soft-deleting inventory items.

## Setup

Requirements: .NET 8 SDK (or newer SDK with the .NET 8 runtime), PostgreSQL 16 or later, and `psql`.

1. Create the database and table using the supplied scripts. Run from the repository root; `inventory_db` must not already exist:

   ```sh
   psql -h localhost -U postgres -d postgres -f database/create-database.sql
   ```

   For an existing database, run `database/01-schema.sql` against `inventory_db` instead. The scripts include the required table, constraints, and indexes.

2. Set your connection string and start the API in PowerShell:

   ```powershell
   $env:ConnectionStrings__Inventory = 'Host=localhost;Port=5432;Database=inventory_db;Username=postgres;Password=YOUR_PASSWORD'
   dotnet run --project src/Inventory.Api --urls http://localhost:8080
   ```

   In Bash, use `export ConnectionStrings__Inventory='...'` before the same `dotnet run` command. Keep your actual password out of source control.

## Endpoints

| Method | Path | Result |
|---|---|---|
| POST | `/api/items` | Create an item; 201 with the item and its Location |
| GET | `/api/items/{id}` | Get an active item; 200 or 404 |
| GET | `/api/items` | List all active items; 200 with a JSON array |
| PUT | `/api/items/{id}` | Update name, category, and quantity; 200 or 404 |
| DELETE | `/api/items/{id}` | Soft-delete an item; 204 or 404 |

POST and PUT accept:

```json
{
  "itemName": "Keyboard",
  "category": "Accessories",
  "quantity": 10
}
```

Responses contain `itemId`, `itemName`, `category`, `quantity`, `isActive`, and `createdAt`. The database generates the ID and creation time; new items are active. The supplied schema's `updated_at` column is set on update/delete.

`itemName` is required (maximum 150 characters), `category` is optional (maximum 100 characters), and `quantity` is required and cannot be negative. Invalid input returns 400. PUT replaces all editable fields; omitting category clears it. Soft deletion retains the database row with `is_active = false`; deleted items are excluded from reads and cannot be updated. A repeated delete returns 404.

## Try the API

Use Postman or the following PowerShell commands. The last read should return 404:

```powershell
$base = 'http://localhost:8080/api/items'
$body = @{ itemName = 'Keyboard'; category = 'Accessories'; quantity = 10 } | ConvertTo-Json
$item = Invoke-RestMethod -Uri $base -Method Post -ContentType 'application/json' -Body $body
Invoke-RestMethod -Uri "$base/$($item.itemId)"
Invoke-RestMethod -Uri $base
$body = @{ itemName = 'Keyboard'; category = 'Accessories'; quantity = 5 } | ConvertTo-Json
Invoke-RestMethod -Uri "$base/$($item.itemId)" -Method Put -ContentType 'application/json' -Body $body
Invoke-RestMethod -Uri "$base/$($item.itemId)" -Method Delete
Invoke-RestMethod -Uri "$base/$($item.itemId)"
```
