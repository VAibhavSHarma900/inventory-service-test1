using System.ComponentModel.DataAnnotations;

namespace Inventory.Api.Contracts;

public sealed class ItemRequest
{
    [Required, StringLength(150)]
    public string ItemName { get; init; } = string.Empty;

    [StringLength(100)]
    public string? Category { get; init; }

    [Required, Range(0, int.MaxValue)]
    public int? Quantity { get; init; }
}
