using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ModeratorWeb.Models;
using ModeratorWeb.Services;

namespace ModeratorWeb.Pages.Players;

public class IndexModel(ApiClient api) : PageModel
{
    public List<UserDto> Users { get; set; } = [];
    public string Search { get; set; } = "";

    public async Task<IActionResult> OnGetAsync(string search = "")
    {
        if (HttpContext.Session.GetString("jwt") is null)
            return RedirectToPage("/Login");

        Search = search;
        var all = await api.GetAllUsersAsync();

        Users = string.IsNullOrWhiteSpace(search)
            ? all
            : all.Where(u =>
                    u.Username.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    u.Email.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    u.Id.ToString().Contains(search, StringComparison.OrdinalIgnoreCase))
                .ToList();

        return Page();
    }

    public async Task<IActionResult> OnPostBlockAsync(Guid userId)
    {
        await api.BlockUserAsync(userId);
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostUnblockAsync(Guid userId)
    {
        await api.UnblockUserAsync(userId);
        return RedirectToPage();
    }
}