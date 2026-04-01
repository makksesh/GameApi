using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ModeratorWeb.Models;
using ModeratorWeb.Services;

namespace ModeratorWeb.Pages;

public class IndexModel(ApiClient api) : PageModel
{
    public List<UserDto> Moderators { get; set; } = [];
    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync()
    {
        if (HttpContext.Session.GetString("jwt") is null)
        {
            Response.Redirect("/Login");
            return;
        }

        try
        {
            var users = await api.GetAllUsersAsync();
            var currentUsername = HttpContext.Session.GetString("username");
            Moderators = users
                .Where(u => u.Role == "Moderator" && u.Username != currentUsername)
                .ToList();
        }
        catch
        {
            ErrorMessage = "Не удалось загрузить список модераторов";
        }
    }

    public async Task<IActionResult> OnPostCreateModeratorAsync(
        string username, string email, string password)
    {
        if (HttpContext.Session.GetString("jwt") is null)
            return RedirectToPage("/Login");

        try
        {
            await api.CreateModeratorAsync(username, email, password);
        }
        catch
        {
            ErrorMessage = "Не удалось создать модератора";
        }

        return RedirectToPage();
    }
}
