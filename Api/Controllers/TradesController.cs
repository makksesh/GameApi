using Api.Extensions;
using Application.DTOs.Trade;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/trades")]
[Authorize]
public class TradesController(TradeService tradeService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetActiveLots(CancellationToken ct)
    {
        var result = await tradeService.GetActiveLotsAsync(ct);
        return Ok(result);
    }

    [HttpPost("lots")]
    public async Task<IActionResult> CreateLot(
        [FromBody] CreateTradeLotRequest request,
        CancellationToken ct)
    {
        var userId = User.GetUserId();
        var result = await tradeService.CreateLotAsync(userId, request, ct);
        return CreatedAtAction(nameof(GetActiveLots), result);
    }

    [HttpPost("buy")]
    public async Task<IActionResult> Buy(
        [FromBody] BuyItemRequest request,
        CancellationToken ct)
    {
        var userId = User.GetUserId();
        var result = await tradeService.BuyItemAsync(userId, request, ct);
        return Ok(result);
    }

    [HttpDelete("lots/{lotId:guid}")]
    public async Task<IActionResult> CancelLot(Guid lotId, CancellationToken ct)
    {
        var userId = User.GetUserId();
        await tradeService.CancelLotAsync(userId, lotId, ct);
        return NoContent();
    }
}