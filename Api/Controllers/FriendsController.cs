using Api.Extensions;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/friends")]
[Authorize]
public class FriendsController(FriendService friendService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetFriends(CancellationToken ct)
    {
        var userId = User.GetUserId();
        var result = await friendService.GetFriendsAsync(userId, ct);
        return Ok(result);
    }
    
    [HttpGet("requests/incoming")]
    public async Task<IActionResult> GetIncomingRequests(CancellationToken ct)
    {
        var userId = User.GetUserId();
        var result = await friendService.GetIncomingRequestsAsync(userId, ct);
        return Ok(result);
    }


    [HttpPost("requests/{receiverUserName}")]
    public async Task<IActionResult> SendRequest(string receiverUserName, CancellationToken ct)
    {
        var senderId = User.GetUserId();
        var receiverId = await friendService.GetUserByNameAsync(receiverUserName, ct);
        var result   = await friendService.SendRequestAsync(senderId, receiverId, ct);
        return Ok(result);
    }

    [HttpPost("requests/{requestId:guid}/accept")]
    public async Task<IActionResult> AcceptRequest(Guid requestId, CancellationToken ct)
    {
        var userId = User.GetUserId();
        await friendService.AcceptRequestAsync(userId, requestId, ct);
        return NoContent();
    }

    [HttpPost("requests/{requestId:guid}/decline")]
    public async Task<IActionResult> DeclineRequest(Guid requestId, CancellationToken ct)
    {
        var userId = User.GetUserId();
        await friendService.DeclineRequestAsync(userId, requestId, ct);
        return NoContent();
    }
}