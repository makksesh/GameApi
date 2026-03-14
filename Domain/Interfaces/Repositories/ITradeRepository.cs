using Domain.Entities;

namespace Domain.Interfaces.Repositories;

public interface ITradeRepository
{
    Task<TradeLot?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<TradeLot>> GetActiveLotsAsync(CancellationToken ct = default);
    Task<IEnumerable<TradeLot>> GetBySellerIdAsync(Guid sellerId, CancellationToken ct = default);
    Task AddAsync(TradeLot lot, CancellationToken ct = default);
    void Update(TradeLot lot);
    Task AddPurchaseAsync(Purchase purchase, CancellationToken ct = default);
}