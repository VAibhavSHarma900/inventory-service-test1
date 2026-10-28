namespace Inventory.Api.Models;

public sealed record InventoryItem(
    Guid ItemId, string ItemName, string? Category, int Quantity,
    bool IsActive, DateTime CreatedAt);
