using Api.Extensions;
using Application.DTOs.Item;
using Application.DTOs.Skill;
using Application.DTOs.User;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/moderator")]
[Authorize(Roles = "Moderator")]
public class ModeratorController(ModeratorService moderatorService) : ControllerBase
{
    // ── SKILLS ────────────────────────────────────────────

    [HttpGet("skills")]
    public async Task<IActionResult> GetSkills(CancellationToken ct)
        => Ok(await moderatorService.GetAllSkillsAsync(ct));

    [HttpPost("skills")]
    public async Task<IActionResult> CreateSkill(
        [FromBody] CreateSkillRequest request, CancellationToken ct)
        => Ok(await moderatorService.CreateSkillAsync(request, ct));

    [HttpPut("skills/{id:guid}")]
    public async Task<IActionResult> UpdateSkill(
        Guid id, [FromBody] UpdateSkillRequest request, CancellationToken ct)
        => Ok(await moderatorService.UpdateSkillAsync(id, request, ct));

    [HttpDelete("skills/{id:guid}")]
    public async Task<IActionResult> DeleteSkill(Guid id, CancellationToken ct)
    {
        await moderatorService.DeleteSkillAsync(id, ct);
        return NoContent();
    }

    // ── ITEMS ─────────────────────────────────────────────

    [HttpGet("items")]
    public async Task<IActionResult> GetItems(CancellationToken ct)
        => Ok(await moderatorService.GetAllItemsAsync(ct));

    [HttpPost("items")]
    public async Task<IActionResult> CreateItem(
        [FromBody] CreateItemRequest request, CancellationToken ct)
        => Ok(await moderatorService.CreateItemAsync(request, ct));

    [HttpPut("items/{id:guid}")]
    public async Task<IActionResult> UpdateItem(
        Guid id, [FromBody] UpdateItemRequest request, CancellationToken ct)
        => Ok(await moderatorService.UpdateItemAsync(id, request, ct));

    [HttpDelete("items/{id:guid}")]
    public async Task<IActionResult> DeleteItem(Guid id, CancellationToken ct)
    {
        await moderatorService.DeleteItemAsync(id, ct);
        return NoContent();
    }

    // ── USERS ─────────────────────────────────────────────

    [HttpDelete("users/{targetId:guid}")]
    public async Task<IActionResult> DeleteUser(Guid targetId, CancellationToken ct)
    {
        var moderatorId = User.GetUserId();
        await moderatorService.DeleteUserAsync(moderatorId, targetId, ct);
        return NoContent();
    }
    
    [HttpPost("users")]
    public async Task<IActionResult> CreateModerator(
        [FromBody] CreateUserDto request,
        CancellationToken ct)
    {
        var moderatorId = User.GetUserId();
        await moderatorService.CreateUserAsync(request, ct);
        return NoContent();
    }
}