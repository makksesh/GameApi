using Api.Extensions;
using Application.DTOs.Character;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/characters")]
[Authorize]
public class CharactersController(CharacterService characterService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateCharacterRequest request,
        CancellationToken ct)
    {
        var userId = User.GetUserId();
        var result = await characterService.CreateAsync(userId, request, ct);
        return CreatedAtAction(nameof(GetMy), result);
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMy(CancellationToken ct)
    {
        var userId = User.GetUserId();
        var result = await characterService.GetByUserIdAsync(userId, ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await characterService.GetByIdAsync(id, ct);
        return Ok(result);
    }

    [HttpPatch("me/stats")]
    public async Task<IActionResult> UpgradeStat(
        [FromBody] UpgradeStatRequest request,
        CancellationToken ct)
    {
        var userId = User.GetUserId();
        var result = await characterService.UpgradeStatAsync(userId, request, ct);
        return Ok(result);
    }
}