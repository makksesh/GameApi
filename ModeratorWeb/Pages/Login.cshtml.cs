using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ModeratorWeb.Services;

namespace ModeratorWeb.Pages;

public class LoginModel(ApiClient api) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new();
    public string? ErrorMessage { get; set; }

    public class InputModel
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Console.WriteLine($"[Login attempt] {Input.Username} / {Input.Password}");

        var result = await api.LoginAsync(Input.Username, Input.Password);

        Console.WriteLine($"[Login result] {(result is null ? "NULL" : result.Role)}");

        if (result is null || result.Role != "Moderator")
        {
            ErrorMessage = result is null
                ? "Неверный логин или пароль"
                : "Доступ разрешён только модераторам";
            return Page();
        }

        HttpContext.Session.SetString("jwt",      result.Token);
        HttpContext.Session.SetString("username", result.Username);
        HttpContext.Session.SetString("userId", result.UserId.ToString());
        HttpContext.Session.SetString("role", result.Role);
        HttpContext.Session.SetString("email",    result.Email); 

        return RedirectToPage("/Index");
    }

    public IActionResult OnPostLogout()
    {
        HttpContext.Session.Clear();
        return RedirectToPage("/Login");
    }
}