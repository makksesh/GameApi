namespace Application.DTOs.Character;

public record CharacterDto(
    Guid Id,
    Guid UserId,
    string Name,
    int Level,
    int Experience,
    int Health,
    int Mana,
    int Armor,
    int Damage,
    decimal Balance,
    DateTime CreatedAt
);

