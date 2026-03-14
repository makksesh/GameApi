using Application.Common.Exceptions;
using Application.DTOs.Character;
using Application.Mappers;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;

namespace Application.Services;

public class CharacterService(
    ICharacterRepository characterRepository,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<CharacterDto> CreateAsync(
        Guid userId,
        CreateCharacterRequest request,
        CancellationToken ct = default)
    {
        var user = await userRepository.GetByIdAsync(userId, ct)
            ?? throw new NotFoundException(nameof(User), userId);
        
        var existing = await characterRepository.GetByUserIdAsync(userId, ct);
        if (existing is not null)
            throw new ConflictException("User already has a character.");
        
        var character = Character.Create(userId, request.Name);
        await characterRepository.AddAsync(character, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return CharacterMapper.ToDto(character);
    }

    public async Task<CharacterDto> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        var character = await characterRepository.GetByUserIdAsync(userId, ct)
            ?? throw new NotFoundException(nameof(Character), userId);

        return CharacterMapper.ToDto(character);
    }

    public async Task<CharacterDto> GetByIdAsync(Guid characterId, CancellationToken ct = default)
    {
        var character = await characterRepository.GetByIdAsync(characterId, ct)
            ?? throw new NotFoundException(nameof(Character), characterId);

        return CharacterMapper.ToDto(character);
    }

    public async Task<CharacterDto> UpgradeStatAsync(
        Guid userId,
        UpgradeStatRequest request,
        CancellationToken ct = default)
    {
        var character = await characterRepository.GetByUserIdAsync(userId, ct)
            ?? throw new NotFoundException(nameof(Character), userId);

        if (request.Amount <= 0)
            throw new BusinessRuleException("Upgrade amount must be positive.");

        character.UpgradeStat(request.Stat, request.Amount);

        characterRepository.Update(character);
        await unitOfWork.SaveChangesAsync(ct);

        return CharacterMapper.ToDto(character);
    }
}