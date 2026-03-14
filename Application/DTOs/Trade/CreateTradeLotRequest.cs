namespace Application.DTOs.Trade;

public record CreateTradeLotRequest(
    Guid ItemId,
    int Quantity,
    decimal Price
);