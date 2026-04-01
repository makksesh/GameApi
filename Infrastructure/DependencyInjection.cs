using Application.Common.Interfaces;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Infrastructure.Security;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // PostgreSQL + EF Core
        services.AddDbContext<GameRpgDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsql => npgsql.MigrationsAssembly(typeof(GameRpgDbContext).Assembly.FullName)));

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Repositories
        services.AddScoped<IUserRepository,      UserRepository>();
        services.AddScoped<ICharacterRepository, CharacterRepository>();
        services.AddScoped<ISkillRepository,     SkillRepository>();
        services.AddScoped<IItemRepository,      ItemRepository>();
        services.AddScoped<IInventoryRepository, InventoryRepository>();
        services.AddScoped<ITradeRepository,     TradeRepository>();
        services.AddScoped<IFriendRepository,    FriendRepository>();
        services.AddScoped<ISupportRepository,   SupportRepository>();
        services.AddScoped<ISupportMessageRepository, SupportMessageRepository>(); 
        services.AddScoped<ICharacterSkillRepository, CharacterSkillRepository>();


        // Security
        services.AddScoped<IPasswordHasher,      PasswordHasher>();
        services.AddScoped<IJwtTokenGenerator,   JwtTokenGenerator>();

        // Services
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        return services;
    }
}