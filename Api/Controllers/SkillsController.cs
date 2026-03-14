using Api.Extensions;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/skills")]
[Authorize]
public class SkillsController(SkillService skillService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await skillService.GetAllSkillsAsync(ct);
        return Ok(result);
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMySkills(CancellationToken ct)
    {
        var userId = User.GetUserId();
        var result = await skillService.GetCharacterSkillsAsync(userId, ct);
        return Ok(result);
    }

    [HttpPost("{skillId:guid}/learn")]
    public async Task<IActionResult> Learn(Guid skillId, CancellationToken ct)
    {
        var userId = User.GetUserId();
        var result = await skillService.LearnSkillAsync(userId, skillId, ct);
        return Ok(result);
    }

    [HttpPost("my/{characterSkillId:guid}/levelup")]
    public async Task<IActionResult> LevelUp(Guid characterSkillId, CancellationToken ct)
    {
        var userId = User.GetUserId();
        var result = await skillService.LevelUpSkillAsync(userId, characterSkillId, ct);
        return Ok(result);
    }
}