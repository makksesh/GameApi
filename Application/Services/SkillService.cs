using Application.Common.Exceptions;
using Application.DTOs.Skill;
using Application.Mappers;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;

namespace Application.Services;

public class SkillService(
    ISkillRepository skillRepository,
    ICharacterRepository characterRepository,
    ICharacterSkillRepository characterSkillRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<IEnumerable<SkillDto>> GetAllSkillsAsync(CancellationToken ct = default)
    {
        var skills = await skillRepository.GetAllAsync(ct);
        return skills.Select(SkillMapper.ToDto);
    }

    public async Task<IEnumerable<CharacterSkillDto>> GetCharacterSkillsAsync(
        Guid userId,
        CancellationToken ct = default)
    {
        var character = await characterRepository.GetByUserIdAsync(userId, ct)
            ?? throw new NotFoundException(nameof(Character), userId);

        return character.Skills.Select(SkillMapper.ToCharacterSkillDto);
    }

    public async Task<CharacterSkillDto> LearnSkillAsync(
        Guid userId,
        Guid skillId,
        CancellationToken ct = default)
    {
        var character = await characterRepository.GetByUserIdAsync(userId, ct)
            ?? throw new NotFoundException(nameof(Character), userId);

        var skill = await skillRepository.GetByIdAsync(skillId, ct)
            ?? throw new NotFoundException(nameof(Skill), skillId);
        
        if (character.Skills.Any(s => s.SkillId == skillId))
            throw new ConflictException($"Skill '{skill.Name}' is already learned.");
        
        if (!character.Balance.IsEnoughFor(skill.LevelUpCost))
            throw new BusinessRuleException("Insufficient balance to learn this skill.");

        character.Withdraw(skill.LevelUpCost);
        var characterSkill = CharacterSkill.Learn(character.Id, skillId);
        await characterSkillRepository.AddAsync(characterSkill, ct);
        Console.WriteLine($"[DEBUG] CharacterSkill Id: {characterSkill.Id}");
        Console.WriteLine($"[DEBUG] CharacterId: {characterSkill.CharacterId}");
        Console.WriteLine($"[DEBUG] SkillId: {characterSkill.SkillId}");
        Console.WriteLine($"[DEBUG] CurrentLevel: {characterSkill.CurrentLevel}");
        await unitOfWork.SaveChangesAsync(ct);
        var saved = await characterSkillRepository.GetByIdAsync(characterSkill.Id, ct)
                    ?? throw new NotFoundException(nameof(CharacterSkill), characterSkill.Id);
        return SkillMapper.ToCharacterSkillDto(
            characterSkill);
    }

    public async Task<CharacterSkillDto> LevelUpSkillAsync(
        Guid userId,
        Guid characterSkillId,
        CancellationToken ct = default)
    {
        var character = await characterRepository.GetByUserIdAsync(userId, ct)
            ?? throw new NotFoundException(nameof(Character), userId);

        var characterSkill = character.Skills.FirstOrDefault(s => s.Id == characterSkillId)
            ?? throw new NotFoundException(nameof(CharacterSkill), characterSkillId);

        if (!character.Balance.IsEnoughFor(characterSkill.Skill.LevelUpCost))
            throw new BusinessRuleException("Insufficient balance to level up this skill.");

        character.Withdraw(characterSkill.Skill.LevelUpCost);
        characterSkill.LevelUp();
        Console.WriteLine($"[DEBUG] CharacterSkill Id: {characterSkill.Id}");
        Console.WriteLine($"[DEBUG] CharacterId: {characterSkill.CharacterId}");
        Console.WriteLine($"[DEBUG] SkillId: {characterSkill.SkillId}");
        Console.WriteLine($"[DEBUG] CurrentLevel: {characterSkill.CurrentLevel}");

        await unitOfWork.SaveChangesAsync(ct);

        return SkillMapper.ToCharacterSkillDto(characterSkill);
    }
}