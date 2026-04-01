using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ModeratorWeb.Models;
using ModeratorWeb.Services;

namespace ModeratorWeb.Pages.Database;

public class IndexModel(ApiClient api) : PageModel
{
    public string Tab    { get; set; } = "skills";
    public string Search { get; set; } = "";

    public List<SkillDto> Skills { get; set; } = [];
    public List<ItemDto>  Items  { get; set; } = [];
    public List<UserDto>  Users  { get; set; } = [];

    public async Task<IActionResult> OnGetAsync(
        string tab = "skills", string search = "")
    {
        if (HttpContext.Session.GetString("jwt") is null)
            return RedirectToPage("/Login");

        Tab    = tab;
        Search = search;

        if (tab == "skills")
            Skills = await api.GetSkillsAsync();

        else if (tab == "items")
            Items = await api.GetItemsAsync();

        else if (tab == "users")
        {
            var all = await api.GetAllUsersAsync();
            Users = string.IsNullOrWhiteSpace(search) ? all
                : all.Where(u =>
                    u.Username.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    u.Email.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    u.Id.ToString().Contains(search, StringComparison.OrdinalIgnoreCase))
                  .ToList();
        }

        return Page();
    }

    // ── Skills ────────────────────────────────────────────
    public async Task<IActionResult> OnPostCreateSkillAsync(
        string name, string description,
        string type, int maxLevel, decimal levelUpCost)
    {
        await api.CreateSkillAsync(
            new CreateSkillRequest(name, description, type, maxLevel, levelUpCost));
        return RedirectToPage(new { tab = "skills" });
    }

    public async Task<IActionResult> OnPostDeleteSkillAsync(Guid id)
    {
        await api.DeleteSkillAsync(id);
        return RedirectToPage(new { tab = "skills" });
    }

    // ── Items ─────────────────────────────────────────────
    public async Task<IActionResult> OnPostCreateItemAsync(
        string itemName, string itemDescription, string itemType,
        string rarity, decimal basePrice,
        int bonusHealth, int bonusMana, int bonusArmor, int bonusDamage)
    {
        await api.CreateItemAsync(new CreateItemRequest(
            itemName, itemDescription, itemType, rarity, basePrice,
            bonusHealth, bonusMana, bonusArmor, bonusDamage));
        return RedirectToPage(new { tab = "items" });
    }

    public async Task<IActionResult> OnPostDeleteItemAsync(Guid id)
    {
        await api.DeleteItemAsync(id);
        return RedirectToPage(new { tab = "items" });
    }

    // ── Users ─────────────────────────────────────────────
    public async Task<IActionResult> OnPostBlockAsync(Guid userId)
    {
        await api.BlockUserAsync(userId);
        return RedirectToPage(new { tab = "users" });
    }

    public async Task<IActionResult> OnPostUnblockAsync(Guid userId)
    {
        await api.UnblockUserAsync(userId);
        return RedirectToPage(new { tab = "users" });
    }

    public async Task<IActionResult> OnPostMakeModeratorAsync(Guid userId)
    {
        await api.ChangeUserRoleAsync(userId, "Moderator");
        return RedirectToPage(new { tab = "users" });
    }

    public async Task<IActionResult> OnPostDeleteUserAsync(Guid userId)
    {
        await api.DeleteUserAsync(userId);
        return RedirectToPage(new { tab = "users" });
    }
}
