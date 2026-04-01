using Api.Extensions;
using Application.DTOs.User;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Moderator")]
public class AdminController(AdminService adminService) : ControllerBase
{
    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers(CancellationToken ct)
    {
        var result = await adminService.GetAllUsersAsync(ct);
        return Ok(result);
    }

    [HttpPost("users/{targetUserId:guid}/block")]
    public async Task<IActionResult> Block(Guid targetUserId, CancellationToken ct)
    {
        var adminId = User.GetUserId();
        var result  = await adminService.BlockUserAsync(adminId, targetUserId, ct);
        return Ok(result);
    }

    [HttpPost("users/{targetUserId:guid}/unblock")]
    public async Task<IActionResult> Unblock(Guid targetUserId, CancellationToken ct)
    {
        var adminId = User.GetUserId();
        var result  = await adminService.UnblockUserAsync(adminId, targetUserId, ct);
        return Ok(result);
    }

    [HttpPatch("users/{targetUserId:guid}/role")]
    public async Task<IActionResult> ChangeRole(
        Guid targetUserId,
        [FromBody] UpdateRoleRequest request,
        CancellationToken ct)
    {
        var adminId = User.GetUserId();
        var result  = await adminService.ChangeUserRoleAsync(adminId, targetUserId, request, ct);
        return Ok(result);
    }
}