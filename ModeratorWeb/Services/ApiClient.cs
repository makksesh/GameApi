using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ModeratorWeb.Models;

namespace ModeratorWeb.Services;

public class ApiClient(HttpClient http, IHttpContextAccessor ctx)
{
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true
    };

    // Подставляем токен из сессии перед каждым запросом
    private void AttachToken()
    {
        var token = ctx.HttpContext?.Session.GetString("jwt");
        if (token is not null)
            http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
    }

    private static StringContent Json<T>(T body) =>
        new(JsonSerializer.Serialize(body),
            Encoding.UTF8, "application/json");

    // ── Auth ──────────────────────────────────────────────
    public async Task<AuthResponse?> LoginAsync(string username, string password)
    {
        var resp = await http.PostAsync("/api/auth/login",
            Json(new LoginRequest(username, password)));

        if (!resp.IsSuccessStatusCode) return null;
        var content = await resp.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<AuthResponse>(content, JsonOpts);
    }

    // ── Support ───────────────────────────────────────────
    public async Task<List<SupportTicketDto>> GetTicketsByStatusAsync(string status)
    {
        AttachToken();
        var resp = await http.GetAsync($"/api/support?status={status}");
        resp.EnsureSuccessStatusCode();
        var content = await resp.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<SupportTicketDto>>(content, JsonOpts)!;
    }

    public async Task AssignTicketAsync(Guid ticketId)
    {
        AttachToken();
        (await http.PostAsync($"/api/support/{ticketId}/assign",
            new StringContent(""))).EnsureSuccessStatusCode();
    }

    public async Task<List<SupportMessageDto>> GetMessagesAsync(Guid ticketId)
    {
        AttachToken();
        var resp = await http.GetAsync($"/api/support/{ticketId}/messages");
        resp.EnsureSuccessStatusCode();
        var content = await resp.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<SupportMessageDto>>(content, JsonOpts)!;
    }

    public async Task SendMessageAsync(Guid ticketId, string text)
    {
        AttachToken();
        (await http.PostAsync($"/api/support/{ticketId}/messages",
            Json(new SendMessageRequest(text)))).EnsureSuccessStatusCode();
    }

    public async Task ResolveTicketAsync(Guid ticketId, string resolution)
    {
        AttachToken();
        (await http.PostAsync($"/api/support/{ticketId}/resolve",
            Json(new ResolveTicketRequest(resolution)))).EnsureSuccessStatusCode();
    }

    // ── Users ─────────────────────────────────────────────
    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        AttachToken();
        var resp = await http.GetAsync("/api/admin/users");
        resp.EnsureSuccessStatusCode();
        var content = await resp.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<UserDto>>(content, JsonOpts)!;
    }

    public async Task BlockUserAsync(Guid userId)
    {
        AttachToken();
        (await http.PostAsync($"/api/admin/users/{userId}/block",
            new StringContent(""))).EnsureSuccessStatusCode();
    }

    public async Task UnblockUserAsync(Guid userId)
    {
        AttachToken();
        (await http.PostAsync($"/api/admin/users/{userId}/unblock",
            new StringContent(""))).EnsureSuccessStatusCode();
    }
    
    // ── Moderator / Skills ────────────────────────────────
    public async Task<List<SkillDto>> GetSkillsAsync()
    {
        AttachToken();
        var resp = await http.GetAsync("/api/moderator/skills");
        resp.EnsureSuccessStatusCode();
        return JsonSerializer.Deserialize<List<SkillDto>>(
            await resp.Content.ReadAsStringAsync(), JsonOpts)!;
    }

    public async Task CreateSkillAsync(CreateSkillRequest request)
    {
        AttachToken();
        (await http.PostAsync("/api/moderator/skills", Json(request)))
            .EnsureSuccessStatusCode();
    }

    public async Task UpdateSkillAsync(Guid id, CreateSkillRequest request)
    {
        AttachToken();
        (await http.PutAsync($"/api/moderator/skills/{id}", Json(request)))
            .EnsureSuccessStatusCode();
    }

    public async Task DeleteSkillAsync(Guid id)
    {
        AttachToken();
        (await http.DeleteAsync($"/api/moderator/skills/{id}"))
            .EnsureSuccessStatusCode();
    }

    // ── Moderator / Items ─────────────────────────────────
    public async Task<List<ItemDto>> GetItemsAsync()
    {
        AttachToken();
        var resp = await http.GetAsync("/api/moderator/items");
        resp.EnsureSuccessStatusCode();
        return JsonSerializer.Deserialize<List<ItemDto>>(
            await resp.Content.ReadAsStringAsync(), JsonOpts)!;
    }

    public async Task CreateItemAsync(CreateItemRequest request)
    {
        AttachToken();
        (await http.PostAsync("/api/moderator/items", Json(request)))
            .EnsureSuccessStatusCode();
    }

    public async Task UpdateItemAsync(Guid id, CreateItemRequest request)
    {
        AttachToken();
        (await http.PutAsync($"/api/moderator/items/{id}", Json(request)))
            .EnsureSuccessStatusCode();
    }

    public async Task DeleteItemAsync(Guid id)
    {
        AttachToken();
        (await http.DeleteAsync($"/api/moderator/items/{id}"))
            .EnsureSuccessStatusCode();
    }

    // ── Moderator / Users (удаление) ──────────────────────
    public async Task DeleteUserAsync(Guid userId)
    {
        AttachToken();
        (await http.DeleteAsync($"/api/moderator/users/{userId}"))
            .EnsureSuccessStatusCode();
    }
    
    public async Task ChangeUserRoleAsync(Guid userId, string role)
    {
        AttachToken();
        (await http.PatchAsync(
                $"/api/admin/users/{userId}/role",
                Json(new { role })))
            .EnsureSuccessStatusCode();
    }

    // ── Moderators ──────────────────────────────────────────
    public async Task CreateModeratorAsync(string username, string email, string password)
    {
        AttachToken();
        (await http.PostAsync("/api/moderator/users",
                Json(new CreateUserDto(username, email, password))))
            .EnsureSuccessStatusCode();
    }

}