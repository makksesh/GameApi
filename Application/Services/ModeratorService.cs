using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.DTOs.Item;
using Application.DTOs.Skill;
using Application.DTOs.User;
using Application.Mappers;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.ValueObjects;

namespace Application.Services;

public class ModeratorService(
    ISkillRepository skillRepository,
    IItemRepository itemRepository,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher)
{
    // ── SKILLS ────────────────────────────────────────────

    public async Task<IEnumerable<SkillDto>> GetAllSkillsAsync(CancellationToken ct = default)
    {
        var skills = await skillRepository.GetAllAsync(ct);
        return skills.Select(SkillMapper.ToDto);
    }

    public async Task<SkillDto> CreateSkillAsync(CreateSkillRequest request, CancellationToken ct = default)
    {
        if (!Enum.TryParse<SkillType>(request.Type, ignoreCase: true, out var type))
            throw new BusinessRuleException($"Unknown skill type: {request.Type}");

        var skill = Skill.Create(
            request.Name, request.Description,
            type, request.MaxLevel,
            new Money(request.LevelUpCost));

        await skillRepository.AddAsync(skill, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return SkillMapper.ToDto(skill);
    }

    public async Task<SkillDto> UpdateSkillAsync(Guid id, UpdateSkillRequest request, CancellationToken ct = default)
    {
        var skill = await skillRepository.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Skill), id);

        if (!Enum.TryParse<SkillType>(request.Type, ignoreCase: true, out var type))
            throw new BusinessRuleException($"Unknown skill type: {request.Type}");

        skill.Update(request.Name, request.Description,
            type, request.MaxLevel, new Money(request.LevelUpCost));

        skillRepository.Update(skill);
        await unitOfWork.SaveChangesAsync(ct);
        return SkillMapper.ToDto(skill);
    }

    public async Task DeleteSkillAsync(Guid id, CancellationToken ct = default)
    {
        var skill = await skillRepository.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Skill), id);

        skillRepository.Delete(skill);
        await unitOfWork.SaveChangesAsync(ct);
    }

    // ── ITEMS ─────────────────────────────────────────────

    public async Task<IEnumerable<ItemDto>> GetAllItemsAsync(CancellationToken ct = default)
    {
        var items = await itemRepository.GetAllAsync(ct);
        return items.Select(ItemMapper.ToDto);
    }

    public async Task<ItemDto> CreateItemAsync(CreateItemRequest request, CancellationToken ct = default)
    {
        if (!Enum.TryParse<ItemType>(request.Type, ignoreCase: true, out var type))
            throw new BusinessRuleException($"Unknown item type: {request.Type}");

        if (!Enum.TryParse<ItemRarity>(request.Rarity, ignoreCase: true, out var rarity))
            throw new BusinessRuleException($"Unknown rarity: {request.Rarity}");

        var bonusStats = new StatBlock(
            request.BonusHealth, request.BonusMana,
            request.BonusArmor, request.BonusDamage);

        var item = Item.Create(request.Name, request.Description,
            type, rarity, new Money(request.BasePrice), bonusStats);

        await itemRepository.AddAsync(item, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return ItemMapper.ToDto(item);
    }

    public async Task<ItemDto> UpdateItemAsync(Guid id, UpdateItemRequest request, CancellationToken ct = default)
    {
        var item = await itemRepository.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Item), id);

        if (!Enum.TryParse<ItemType>(request.Type, ignoreCase: true, out var type))
            throw new BusinessRuleException($"Unknown item type: {request.Type}");

        if (!Enum.TryParse<ItemRarity>(request.Rarity, ignoreCase: true, out var rarity))
            throw new BusinessRuleException($"Unknown rarity: {request.Rarity}");

        var bonusStats = new StatBlock(
            request.BonusHealth, request.BonusMana,
            request.BonusArmor, request.BonusDamage);

        item.Update(request.Name, request.Description,
            type, rarity, new Money(request.BasePrice), bonusStats);

        itemRepository.Update(item);
        await unitOfWork.SaveChangesAsync(ct);
        return ItemMapper.ToDto(item);
    }

    public async Task DeleteItemAsync(Guid id, CancellationToken ct = default)
    {
        var item = await itemRepository.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Item), id);

        itemRepository.Delete(item);
        await unitOfWork.SaveChangesAsync(ct);
    }

    // ── USERS ─────────────────────────────────────────────

    public async Task DeleteUserAsync(Guid moderatorId, Guid targetId, CancellationToken ct = default)
    {
        var moderator = await userRepository.GetByIdAsync(moderatorId, ct)
            ?? throw new NotFoundException(nameof(User), moderatorId);

        if (moderator.Role != UserRole.Moderator)
            throw new ForbiddenException();

        var target = await userRepository.GetByIdAsync(targetId, ct)
            ?? throw new NotFoundException(nameof(User), targetId);

        if (target.Role == UserRole.Moderator)
            throw new BusinessRuleException("Cannot delete another moderator.");

        userRepository.Delete(target);
        await unitOfWork.SaveChangesAsync(ct);
    }
    
    public async Task CreateUserAsync(CreateUserDto request, CancellationToken ct = default)
    {
        if (await userRepository.GetByUsernameAsync(request.Username, ct) is not null)
            throw new ConflictException($"Username '{request.Username}' is already taken.");

        if (await userRepository.GetByEmailAsync(request.Email, ct) is not null)
            throw new ConflictException($"Email '{request.Email}' is already registered.");

        // Хэшируем через интерфейс, не через BCrypt напрямую
        var passwordHash = passwordHasher.Hash(request.Password);

        var user = User.Create(
            username:     request.Username,
            email:        new Email(request.Email),
            passwordHash: passwordHash
        );

        // Role назначается отдельно, если фабрика её не принимает
        user.ChangeRole(UserRole.Moderator); // или user.Role = request.Role; если сеттер открыт

        await userRepository.AddAsync(user, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }

}