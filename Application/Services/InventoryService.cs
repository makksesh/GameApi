using Application.Common.Exceptions;
using Application.DTOs.Inventory;
using Application.Mappers;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;

namespace Application.Services;

public class InventoryService(
    ICharacterRepository characterRepository,
    IInventoryRepository inventoryRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<IEnumerable<InventoryItemDto>> GetInventoryAsync(
        Guid userId,
        CancellationToken ct = default)
    {
        var character = await characterRepository.GetByUserIdAsync(userId, ct)
            ?? throw new NotFoundException(nameof(Character), userId);

        var items = await inventoryRepository.GetByCharacterIdAsync(character.Id, ct);
        return items.Select(InventoryMapper.ToDto);
    }

    public async Task<InventoryItemDto> EquipItemAsync(
        Guid userId,
        Guid inventoryItemId,
        CancellationToken ct = default)
    {
        var character = await characterRepository.GetByUserIdAsync(userId, ct)
            ?? throw new NotFoundException(nameof(Character), userId);

        var item = await inventoryRepository.GetByIdAsync(inventoryItemId, ct)
            ?? throw new NotFoundException(nameof(InventoryItem), inventoryItemId);
        
        if (item.CharacterId != character.Id)
            throw new ForbiddenException("This item does not belong to your character.");

        if (item.IsEquipped)
            throw new BusinessRuleException("Item is already equipped.");

        item.Equip();
        inventoryRepository.Update(item);
        await unitOfWork.SaveChangesAsync(ct);

        return InventoryMapper.ToDto(item);
    }

    public async Task<InventoryItemDto> UnequipItemAsync(
        Guid userId,
        Guid inventoryItemId,
        CancellationToken ct = default)
    {
        var character = await characterRepository.GetByUserIdAsync(userId, ct)
            ?? throw new NotFoundException(nameof(Character), userId);

        var item = await inventoryRepository.GetByIdAsync(inventoryItemId, ct)
            ?? throw new NotFoundException(nameof(InventoryItem), inventoryItemId);

        if (item.CharacterId != character.Id)
            throw new ForbiddenException("This item does not belong to your character.");

        if (!item.IsEquipped)
            throw new BusinessRuleException("Item is not equipped.");

        item.Unequip();
        inventoryRepository.Update(item);
        await unitOfWork.SaveChangesAsync(ct);

        return InventoryMapper.ToDto(item);
    }
}