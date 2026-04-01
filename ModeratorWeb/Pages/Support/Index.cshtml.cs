using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ModeratorWeb.Models;
using ModeratorWeb.Services;

namespace ModeratorWeb.Pages.Support;

public class IndexModel(ApiClient api) : PageModel
{
    public List<SupportTicketDto> Tickets { get; set; } = [];
    public string Status { get; set; } = "New";

    public async Task<IActionResult> OnGetAsync(string status = "New")
    {
        if (HttpContext.Session.GetString("jwt") is null)
            return RedirectToPage("/Login");

        Status = status;
        Tickets = await api.GetTicketsByStatusAsync(status);
        return Page();
    }

    public async Task<IActionResult> OnPostAssignAsync(Guid ticketId)
    {
        await api.AssignTicketAsync(ticketId);
        return RedirectToPage(new { status = "InProgress" });
    }
}