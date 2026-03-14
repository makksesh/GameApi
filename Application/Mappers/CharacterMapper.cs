using Application.DTOs.Character;
using Domain.Entities;

namespace Application.Mappers;

public static class CharacterMapper
{
    public static CharacterDto ToDto(Character character)
        => new(
            Id:         character.Id,
            UserId:     character.UserId,
            Name:       character.Name,
            Level:      character.Level,
            Experience: character.Experience,
            Health:     character.Stats.Health,
            Mana:       character.Stats.Mana,
            Armor:      character.Stats.Armor,
            Damage:     character.Stats.Damage,
            Balance:    character.Balance.Amount,
            CreatedAt:  character.CreatedAt
        );
}