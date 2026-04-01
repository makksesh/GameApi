using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public static class DevSeeder
{
    static DateTime UtcDate(int year, int month, int day) =>
        new(year, month, day, 0, 0, 0, DateTimeKind.Utc);

    public static async Task SeedAsync(GameRpgDbContext context, CancellationToken ct = default)
    {
        if ((await context.Database.GetPendingMigrationsAsync(ct)).Any())
            return;

        if (await context.Users.AnyAsync(ct))
            return;

        await using var transaction = await context.Database.BeginTransactionAsync(ct);

        try
        {
            // ─── Users ────────────────────────────────────────────────────
            var hasher = new Infrastructure.Security.PasswordHasher();

            var admin = User.Create("Admin", new Email("admin@example.com"),
                hasher.Hash("Admin"), null);
            admin.ChangeRole(UserRole.Moderator);

            var moderator = User.Create("Mod", new Email("moderator@example.com"),
                hasher.Hash("pass123"), null);
            moderator.ChangeRole(UserRole.Moderator);

            var player1 = User.Create("Gamer", new Email("gamer@example.com"),
                hasher.Hash("pass123"), UtcDate(1998, 5, 14));

            var player2 = User.Create("Player", new Email("player@example.com"),
                hasher.Hash("pass123"), UtcDate(2000, 3, 22));

            var player3 = User.Create("Unknown", new Email("unknown@example.com"),
                hasher.Hash("pass123"), UtcDate(1995, 11, 7));

            await context.Users.AddRangeAsync(admin, moderator, player1, player2, player3);
            await context.SaveChangesAsync(ct);

            // Characters
            var johnChar      = Character.Create(player1.Id,   "Babochka");
            var aliceChar     = Character.Create(player2.Id,   "Klire");
            var bobChar       = Character.Create(player3.Id,   "ShadowBob");

            await context.Characters.AddRangeAsync(johnChar, aliceChar, bobChar);
            await context.SaveChangesAsync(ct);

            // Skills
            var fireball = Skill.Create(
                name: "Fireball",
                description: "Launches a blazing fireball at the enemy.",
                type: SkillType.Active,
                maxLevel: 5,
                levelUpCost: new Money(50));

            var frostBolt = Skill.Create(
                name: "Frost Bolt",
                description: "Fires an icy bolt that slows enemies.",
                type: SkillType.Active,
                maxLevel: 5,
                levelUpCost: new Money(45));

            var shadowStrike = Skill.Create(
                name: "Shadow Strike",
                description: "A quick backstab dealing massive damage.",
                type: SkillType.Active,
                maxLevel: 4,
                levelUpCost: new Money(60));

            var toughSkin = Skill.Create(
                name: "Tough Skin",
                description: "Permanently increases armor rating.",
                type: SkillType.Passive,
                maxLevel: 3,
                levelUpCost: new Money(30));

            var ironWill = Skill.Create(
                name: "Iron Will",
                description: "Boosts max health through sheer willpower.",
                type: SkillType.Passive,
                maxLevel: 5,
                levelUpCost: new Money(40));

            var battleCry = Skill.Create(
                name: "Battle Cry",
                description: "Rallies the fighter, temporarily boosting all stats.",
                type: SkillType.Active,
                maxLevel: 3,
                levelUpCost: new Money(55));

            await context.Skills.AddRangeAsync(fireball, frostBolt, shadowStrike,
                                               toughSkin, ironWill, battleCry);
            await context.SaveChangesAsync(ct);

            // Items
            var ironSword = Item.Create(
                name: "Iron Sword",
                description: "A reliable basic melee weapon.",
                type: ItemType.Weapon,
                rarity: ItemRarity.Common,
                basePrice: new Money(20),
                bonusStats: new StatBlock(1, 0, 0, 5));

            var steelAxe = Item.Create(
                name: "Steel Axe",
                description: "Heavy axe for seasoned warriors.",
                type: ItemType.Weapon,
                rarity: ItemRarity.Uncommon,
                basePrice: new Money(45),
                bonusStats: new StatBlock(1, 0, 0, 12));

            var leatherArmor = Item.Create(
                name: "Leather Armor",
                description: "Light armor suited for beginners.",
                type: ItemType.Armor,
                rarity: ItemRarity.Common,
                basePrice: new Money(25),
                bonusStats: new StatBlock(20, 0, 5, 1));

            var chainmail = Item.Create(
                name: "Chainmail",
                description: "Sturdy mid-tier armor with solid protection.",
                type: ItemType.Armor,
                rarity: ItemRarity.Uncommon,
                basePrice: new Money(60),
                bonusStats: new StatBlock(40, 0, 12, 1));

            var healthPotion = Item.Create(
                name: "Health Potion",
                description: "Restores a moderate amount of health.",
                type: ItemType.Potion,
                rarity: ItemRarity.Common,
                basePrice: new Money(5),
                bonusStats: null);

            var elixirOfPower = Item.Create(
                name: "Elixir of Power",
                description: "Rare brew that permanently boosts damage.",
                type: ItemType.Potion,
                rarity: ItemRarity.Rare,
                basePrice: new Money(120),
                bonusStats: new StatBlock(1, 10, 0, 8));

            await context.Items.AddRangeAsync(ironSword, steelAxe, leatherArmor,
                                              chainmail, healthPotion, elixirOfPower);
            await context.SaveChangesAsync(ct);

            // Inventories
            
            await context.InventoryItems.AddRangeAsync(
                InventoryItem.Create(johnChar.Id, ironSword.Id,    1),
                InventoryItem.Create(johnChar.Id, leatherArmor.Id, 1),
                InventoryItem.Create(johnChar.Id, healthPotion.Id, 3));
            
            await context.InventoryItems.AddRangeAsync(
                InventoryItem.Create(aliceChar.Id, elixirOfPower.Id, 2),
                InventoryItem.Create(aliceChar.Id, healthPotion.Id,  5),
                InventoryItem.Create(aliceChar.Id, ironSword.Id,    1),
                InventoryItem.Create(aliceChar.Id, leatherArmor.Id, 1));
            
            await context.InventoryItems.AddRangeAsync(
                InventoryItem.Create(bobChar.Id, steelAxe.Id,    1),
                InventoryItem.Create(bobChar.Id, chainmail.Id,   1),
                InventoryItem.Create(bobChar.Id, healthPotion.Id, 2));

            await context.SaveChangesAsync(ct);

            // Trade Lots
            await context.TradeLots.AddAsync(
                TradeLot.Create(
                    sellerId: johnChar.Id,
                    itemId: healthPotion.Id,
                    quantity: 2,
                    price: new Money(8)), ct);
            
            await context.TradeLots.AddAsync(
                TradeLot.Create(
                    sellerId: aliceChar.Id,
                    itemId: elixirOfPower.Id,
                    quantity: 1,
                    price: new Money(150)), ct);
            
            await context.TradeLots.AddAsync(
                TradeLot.Create(
                    sellerId: bobChar.Id,
                    itemId: ironSword.Id,
                    quantity: 1,
                    price: new Money(15)), ct);

            await context.SaveChangesAsync(ct);

            await transaction.CommitAsync(ct);
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }
}
