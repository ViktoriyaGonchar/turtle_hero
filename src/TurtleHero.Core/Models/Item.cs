namespace TurtleHero.Core.Models;

/// <summary>
/// Тип предмета
/// </summary>
public enum ItemType
{
    Consumable,  // Потребляемый (зелье, еда)
    Weapon,      // Оружие
    Armor,       // Броня
    Quest        // Квестовый предмет
}

/// <summary>
/// Предмет в игре
/// </summary>
public class Item
{
    public Item() { } // Конструктор без параметров для сериализации
    
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Emoji { get; set; } = "📦";
    public string Description { get; set; } = string.Empty;
    public ItemType Type { get; set; } = ItemType.Consumable;
    
    // Боевые бонусы
    public int StrengthBonus { get; set; } = 0;
    public int DefenseBonus { get; set; } = 0;
    public int AgilityBonus { get; set; } = 0;
    
    // Эффекты для потребляемых предметов
    public int HealthRestore { get; set; } = 0;
    public int AgilityBoost { get; set; } = 0; // Временный бонус к ловкости на бой
    
    // Для отображения
    public int MaxStack { get; set; } = 99; // Максимальный размер стака
}

/// <summary>
/// Стак предметов в инвентаре
/// </summary>
public class ItemStack
{
    public Item Item { get; set; } = null!;
    public int Quantity { get; set; }
    
    public ItemStack() { } // Конструктор без параметров для сериализации
    
    public ItemStack(Item item, int quantity = 1)
    {
        Item = item;
        Quantity = quantity;
    }
    
    /// <summary>
    /// Можно ли добавить ещё предметов в этот стак
    /// </summary>
    public bool CanAdd(int amount) => Item != null && Quantity + amount <= Item.MaxStack;
    
    /// <summary>
    /// Добавляет предметы в стак
    /// </summary>
    public void Add(int amount)
    {
        if (CanAdd(amount))
        {
            Quantity += amount;
        }
    }
    
    /// <summary>
    /// Удаляет предметы из стака
    /// </summary>
    public bool Remove(int amount)
    {
        if (Quantity >= amount)
        {
            Quantity -= amount;
            return true;
        }
        return false;
    }
}

