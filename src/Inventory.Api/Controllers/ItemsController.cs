using Inventory.Api.Contracts;
using Inventory.Api.Models;
using Inventory.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Api.Controllers;

[ApiController]
[Route("api/items")]
public sealed class ItemsController(InventoryRepository repository) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<InventoryItem>> Create(ItemRequest request, CancellationToken ct)
    {
        var item = await repository.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = item.ItemId }, item);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<InventoryItem>> GetById(Guid id, CancellationToken ct)
    {
        var item = await repository.GetAsync(id, ct);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<InventoryItem>>> List(CancellationToken ct)
        => Ok(await repository.ListAsync(ct));

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<InventoryItem>> Update(Guid id, ItemRequest request, CancellationToken ct)
    {
        var item = await repository.UpdateAsync(id, request, ct);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        => await repository.SoftDeleteAsync(id, ct) ? NoContent() : NotFound();
}
