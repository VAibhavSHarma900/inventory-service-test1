using Inventory.Api.Contracts;
using Inventory.Api.Models;
using Npgsql;
using NpgsqlTypes;

namespace Inventory.Api.Repositories;

public sealed class InventoryRepository(NpgsqlDataSource dataSource)
{
    private const string Columns = "item_id, item_name, category, quantity, is_active, created_at";

    public async Task<InventoryItem> CreateAsync(ItemRequest request, CancellationToken ct)
    {
        await using var cmd = dataSource.CreateCommand($"""
            INSERT INTO inventory_items (item_name, category, quantity)
            VALUES (@name, @category, @quantity) RETURNING {Columns}
            """);
        AddValues(cmd, request);
        return (await ReadOneAsync(cmd, ct))!;
    }

    public async Task<InventoryItem?> GetAsync(Guid id, CancellationToken ct)
    {
        await using var cmd = dataSource.CreateCommand($"SELECT {Columns} FROM inventory_items WHERE item_id = @id AND is_active");
        cmd.Parameters.AddWithValue("id", id);
        return await ReadOneAsync(cmd, ct);
    }

    public async Task<IReadOnlyList<InventoryItem>> ListAsync(CancellationToken ct)
    {
        await using var cmd = dataSource.CreateCommand($"""
            SELECT {Columns} FROM inventory_items WHERE is_active
            ORDER BY created_at DESC, item_id DESC
            """);
        var items = new List<InventoryItem>();
        await using var reader = await cmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct)) items.Add(Map(reader));
        return items;
    }

    public async Task<InventoryItem?> UpdateAsync(Guid id, ItemRequest request, CancellationToken ct)
    {
        await using var cmd = dataSource.CreateCommand($"""
            UPDATE inventory_items SET item_name = @name, category = @category,
                quantity = @quantity, updated_at = CURRENT_TIMESTAMP
            WHERE item_id = @id AND is_active RETURNING {Columns}
            """);
        cmd.Parameters.AddWithValue("id", id);
        AddValues(cmd, request);
        return await ReadOneAsync(cmd, ct);
    }

    public async Task<bool> SoftDeleteAsync(Guid id, CancellationToken ct)
    {
        await using var cmd = dataSource.CreateCommand("""
            UPDATE inventory_items SET is_active = FALSE, updated_at = CURRENT_TIMESTAMP
            WHERE item_id = @id AND is_active
            """);
        cmd.Parameters.AddWithValue("id", id);
        return await cmd.ExecuteNonQueryAsync(ct) == 1;
    }

    private static void AddValues(NpgsqlCommand cmd, ItemRequest request)
    {
        cmd.Parameters.AddWithValue("name", request.ItemName.Trim());
        cmd.Parameters.Add("category", NpgsqlDbType.Varchar).Value =
            string.IsNullOrWhiteSpace(request.Category) ? DBNull.Value : request.Category.Trim();
        cmd.Parameters.AddWithValue("quantity", request.Quantity!.Value);
    }

    private static async Task<InventoryItem?> ReadOneAsync(NpgsqlCommand cmd, CancellationToken ct)
    {
        await using var reader = await cmd.ExecuteReaderAsync(ct);
        return await reader.ReadAsync(ct) ? Map(reader) : null;
    }

    private static InventoryItem Map(NpgsqlDataReader reader) => new(
        reader.GetGuid(0), reader.GetString(1), reader.IsDBNull(2) ? null : reader.GetString(2),
        reader.GetInt32(3), reader.GetBoolean(4), reader.GetDateTime(5));
}
