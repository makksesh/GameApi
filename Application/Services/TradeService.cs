using Application.Common.Exceptions;
using Application.DTOs.Trade;
using Application.Mappers;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.ValueObjects;

namespace Application.Services;

public class TradeService(
    ITradeRepository tradeRepository,
    ICharacterRepository characterRepository,
    IInventoryRepository inventoryRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<IEnumerable<TradeLotDto>> GetActiveLotsAsync(CancellationToken ct = default)
    {
        var lots = await tradeRepository.GetActiveLotsAsync(ct);
        return lots.Select(TradeMapper.ToDto);
    }

    public async Task<TradeLotDto> CreateLotAsync(
        Guid userId,
        CreateTradeLotRequest request,
        CancellationToken ct = default)
    {
        var character = await characterRepository.GetByUserIdAsync(userId, ct)
            ?? throw new NotFoundException(nameof(Character), userId);
        Console.WriteLine(character.Id);

        var inventoryItem = await inventoryRepository
            .GetByCharacterAndItemAsync(character.Id, request.ItemId, ct)
            ?? throw new BusinessRuleException("Item not found in your inventory.");

        if (inventoryItem.Quantity < request.Quantity)
            throw new BusinessRuleException(
                $"Not enough items: have {inventoryItem.Quantity}, need {request.Quantity}.");

        if (inventoryItem.IsEquipped)
            throw new BusinessRuleException("Cannot sell equipped item. Unequip it first.");


        inventoryItem.RemoveQuantity(request.Quantity);
        if (inventoryItem.Quantity == 0)
            inventoryRepository.Delete(inventoryItem);
        else
            inventoryRepository.Update(inventoryItem);


        var lot = TradeLot.Create(character.Id, request.ItemId, request.Quantity, new Money(request.Price));
        await tradeRepository.AddAsync(lot, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return TradeMapper.ToDto(lot);
    }

    public async Task<TradeLotDto> BuyItemAsync(
        Guid userId,
        BuyItemRequest request,
        CancellationToken ct = default)
    {
        await unitOfWork.BeginTransactionAsync(ct);
        try
        {
            var buyer = await characterRepository.GetByUserIdAsync(userId, ct)
                ?? throw new NotFoundException(nameof(Character), userId);

            var lot = await tradeRepository.GetByIdAsync(request.TradeLotId, ct)
                ?? throw new NotFoundException(nameof(TradeLot), request.TradeLotId);

            if (lot.SellerId == buyer.Id)
                throw new BusinessRuleException("Cannot buy your own lot.");

            if (!buyer.Balance.IsEnoughFor(lot.Price))
                throw new BusinessRuleException("Insufficient balance.");


            buyer.Withdraw(lot.Price);


            var seller = await characterRepository.GetByIdAsync(lot.SellerId, ct)
                ?? throw new NotFoundException(nameof(Character), lot.SellerId);
            seller.Deposit(lot.Price);


            var existingItem = await inventoryRepository
                .GetByCharacterAndItemAsync(buyer.Id, lot.ItemId, ct);

            if (existingItem is not null)
            {
                existingItem.AddQuantity(lot.Quantity);
                inventoryRepository.Update(existingItem);
            }
            else
            {
                var newItem = InventoryItem.Create(buyer.Id, lot.ItemId, lot.Quantity);
                await inventoryRepository.AddAsync(newItem, ct);
            }


            lot.MarkAsSold();
            tradeRepository.Update(lot);

            var purchase = Purchase.Create(buyer.Id, lot.Id, lot.Quantity, lot.Price);
            await tradeRepository.AddPurchaseAsync(purchase, ct);

            characterRepository.Update(buyer);
            characterRepository.Update(seller);

            await unitOfWork.SaveChangesAsync(ct);
            await unitOfWork.CommitTransactionAsync(ct);

            return TradeMapper.ToDto(lot);
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync(ct);
            throw;
        }
    }

    public async Task CancelLotAsync(Guid userId, Guid lotId, CancellationToken ct = default)
    {
        var character = await characterRepository.GetByUserIdAsync(userId, ct)
            ?? throw new NotFoundException(nameof(Character), userId);

        var lot = await tradeRepository.GetByIdAsync(lotId, ct)
            ?? throw new NotFoundException(nameof(TradeLot), lotId);

        if (lot.SellerId != character.Id)
            throw new ForbiddenException("Cannot cancel someone else's lot.");

        lot.Cancel();


        var existingItem = await inventoryRepository
            .GetByCharacterAndItemAsync(character.Id, lot.ItemId, ct);

        if (existingItem is not null)
        {
            existingItem.AddQuantity(lot.Quantity);
            inventoryRepository.Update(existingItem);
        }
        else
        {
            var restoredItem = InventoryItem.Create(character.Id, lot.ItemId, lot.Quantity);
            await inventoryRepository.AddAsync(restoredItem, ct);
        }

        tradeRepository.Update(lot);
        await unitOfWork.SaveChangesAsync(ct);
    }
}