using System.Collections.Generic;

namespace TurtleHero.Core.Models;

/// <summary>
/// Инвентарь персонажа
/// </summary>
public class Inventory
{
    private Dictionary<string, ItemStack> _items = new();
    public const int MaxSlots = 12;
    
    /// <summary>
    /// Все предметы в инвентаре (для сериализации)
    /// </summary>
    public Dictionary<string, ItemStack> Items
    {
        get => _items;
        set => _items = value ?? new Dictionary<string, ItemStack>();
    }
    
    /// <summary>
    /// Количество занятых слотов
    /// </summary>
    public int UsedSlots => _items.Count;
    
    /// <summary>
    /// Есть ли свободные слоты
    /// </summary>
    public bool HasFreeSlots => UsedSlots < MaxSlots;
    
    /// <summary>
    /// Добавляет предмет в инвентарь
    /// </summary>
    public bool AddItem(Item item, int quantity = 1)
    {
        if (item == null || quantity <= 0) return false;
        
        if (_items.TryGetValue(item.Id, out var existingStack))
        {
            // Если предмет уже есть, пытаемся добавить в существующий стак
            if (existingStack.CanAdd(quantity))
            {
                existingStack.Add(quantity);
                return true;
            }
            // Если стак заполнен, пытаемся создать новый стак (если есть слот)
            if (HasFreeSlots && quantity <= item.MaxStack)
            {
                _items.Add($"{item.Id}_{_items.Count}", new ItemStack(item, quantity));
                return true;
            }
            return false;
        }
        else
        {
            // Новый предмет - нужен свободный слот
            if (!HasFreeSlots) return false;
            
            _items.Add(item.Id, new ItemStack(item, quantity));
            return true;
        }
    }
    
    /// <summary>
    /// Удаляет предмет из инвентаря
    /// </summary>
    public bool RemoveItem(string itemId, int quantity = 1)
    {
        if (string.IsNullOrEmpty(itemId) || quantity <= 0) return false;
        
        if (_items.TryGetValue(itemId, out var stack))
        {
            if (stack.Remove(quantity))
            {
                if (stack.Quantity <= 0)
                {
                    _items.Remove(itemId);
                }
                return true;
            }
        }
        
        return false;
    }
    
    /// <summary>
    /// Получает количество предмета в инвентаре
    /// </summary>
    public int GetItemCount(string itemId)
    {
        if (_items.TryGetValue(itemId, out var stack))
        {
            return stack.Quantity;
        }
        return 0;
    }
    
    /// <summary>
    /// Проверяет, есть ли предмет в инвентаре
    /// </summary>
    public bool HasItem(string itemId, int quantity = 1)
    {
        return GetItemCount(itemId) >= quantity;
    }
    
    /// <summary>
    /// Получает стак предмета
    /// </summary>
    public ItemStack? GetItemStack(string itemId)
    {
        return _items.TryGetValue(itemId, out var stack) ? stack : null;
    }
    
    /// <summary>
    /// Очищает инвентарь
    /// </summary>
    public void Clear()
    {
        _items.Clear();
    }
}

