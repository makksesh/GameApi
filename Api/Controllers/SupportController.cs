using Api.Extensions;
using Application.DTOs.Support;
using Application.Services;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/support")]
[Authorize]
public class SupportController(SupportService supportService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateTicket(
        [FromBody] CreateTicketRequest request,
        CancellationToken ct)
    {
        var userId = User.GetUserId();
        var result = await supportService.CreateTicketAsync(userId, request, ct);
        return Ok(result);
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyTickets(CancellationToken ct)
    {
        var userId = User.GetUserId();
        var result = await supportService.GetMyTicketsAsync(userId, ct);
        return Ok(result);
    }

    [HttpPost("{ticketId:guid}/close")]
    public async Task<IActionResult> Close(Guid ticketId, CancellationToken ct)
    {
        var userId = User.GetUserId();
        await supportService.CloseTicketAsync(userId, ticketId, ct);
        return NoContent();
    }

    [HttpPost("{ticketId:guid}/reopen")]
    public async Task<IActionResult> Reopen(Guid ticketId, CancellationToken ct)
    {
        var userId = User.GetUserId();
        await supportService.ReopenTicketAsync(userId, ticketId, ct);
        return NoContent();
    }


    #region Moderator endpoints

    [HttpGet]
    [Authorize(Roles = "Moderator")]
    public async Task<IActionResult> GetByStatus(
        [FromQuery] SupportStatus status = SupportStatus.New,
        CancellationToken ct = default)
    {
        var result = await supportService.GetTicketsByStatusAsync(status, ct);
        return Ok(result);
    }

    [HttpPost("{ticketId:guid}/assign")]
    [Authorize(Roles = "Moderator")]
    public async Task<IActionResult> Assign(Guid ticketId, CancellationToken ct)
    {
        var moderatorId = User.GetUserId();
        var result      = await supportService.AssignModeratorAsync(moderatorId, ticketId, ct);
        return Ok(result);
    }

    [HttpPost("{ticketId:guid}/resolve")]
    [Authorize(Roles = "Moderator")]
    public async Task<IActionResult> Resolve(
        Guid ticketId,
        [FromBody] ResolveTicketRequest request,
        CancellationToken ct)
    {
        var moderatorId = User.GetUserId();
        var result      = await supportService.ResolveTicketAsync(moderatorId, ticketId, request, ct);
        return Ok(result);
    }
    
    [HttpGet("{ticketId:guid}/messages")]
    public async Task<IActionResult> GetMessages(Guid ticketId, CancellationToken ct)
    {
        var userId      = User.GetUserId();
        var isModerator = User.IsInRole("Moderator");
        var messages    = await supportService.GetMessagesAsync(userId, isModerator, ticketId, ct);
        return Ok(messages);
    }

    [HttpPost("{ticketId:guid}/messages")]
    public async Task<IActionResult> SendMessage(
        Guid ticketId, [FromBody] SendMessageRequest request, CancellationToken ct)
    {
        var userId      = User.GetUserId();
        var isModerator = User.IsInRole("Moderator") || User.IsInRole("Admin"); // тоже фикс №4
        var message     = await supportService.SendMessageAsync(userId, isModerator, ticketId, request, ct);
        return Ok(message);
    }

    #endregion
}