namespace Application.DTOs.Character;

public record UpgradeStatRequest(
    string Stat,
    int Amount
);