using Application.Services;

namespace Api.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<AuthService>();
        services.AddScoped<CharacterService>();
        services.AddScoped<SkillService>();
        services.AddScoped<InventoryService>();
        services.AddScoped<TradeService>();
        services.AddScoped<FriendService>();
        services.AddScoped<SupportService>();
        services.AddScoped<AdminService>();

        return services;
    }
}