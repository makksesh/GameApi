using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ModeratorWeb.Models;
using ModeratorWeb.Services;

namespace ModeratorWeb.Pages.Support;

public class ChatModel(ApiClient api) : PageModel
{
    public List<SupportMessageDto> Messages { get; set; } = [];
    public Guid TicketId { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid ticketId)
    {
        if (HttpContext.Session.GetString("jwt") is null)
            return RedirectToPage("/Login");

        TicketId = ticketId;
        Messages = await api.GetMessagesAsync(ticketId);
        return Page();
    }

    public async Task<IActionResult> OnPostSendAsync(Guid ticketId, string text)
    {
        await api.SendMessageAsync(ticketId, text);
        return RedirectToPage(new { ticketId });
    }

    public async Task<IActionResult> OnPostResolveAsync(Guid ticketId, string resolution)
    {
        await api.ResolveTicketAsync(ticketId, resolution);
        return RedirectToPage("/Support/Index");
    }
}