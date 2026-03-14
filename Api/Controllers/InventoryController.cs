using Api.Extensions;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/inventory")]
[Authorize]
public class InventoryController(InventoryService inventoryService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetMyInventory(CancellationToken ct)
    {
        var userId = User.GetUserId();
        var result = await inventoryService.GetInventoryAsync(userId, ct);
        return Ok(result);
    }

    [HttpPost("{inventoryItemId:guid}/equip")]
    public async Task<IActionResult> Equip(Guid inventoryItemId, CancellationToken ct)
    {
        var userId = User.GetUserId();
        var result = await inventoryService.EquipItemAsync(userId, inventoryItemId, ct);
        return Ok(result);
    }

    [HttpPost("{inventoryItemId:guid}/unequip")]
    public async Task<IActionResult> Unequip(Guid inventoryItemId, CancellationToken ct)
    {
        var userId = User.GetUserId();
        var result = await inventoryService.UnequipItemAsync(userId, inventoryItemId, ct);
        return Ok(result);
    }
}