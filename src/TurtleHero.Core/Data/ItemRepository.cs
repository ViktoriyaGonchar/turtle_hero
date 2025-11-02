using TurtleHero.Core.Models;

namespace TurtleHero.Core.Data;

/// <summary>
/// Репозиторий предметов игры
/// </summary>
public class ItemRepository
{
    private readonly Dictionary<string, Item> _items = new();
    
    public ItemRepository()
    {
        InitializeDefaultItems();
    }
    
    /// <summary>
    /// Получает предмет по ID
    /// </summary>
    public Item? GetItem(string itemId)
    {
        return _items.TryGetValue(itemId, out var item) ? item : null;
    }
    
    /// <summary>
    /// Получает все предметы
    /// </summary>
    public IEnumerable<Item> GetAllItems() => _items.Values;
    
    /// <summary>
    /// Регистрирует предмет
    /// </summary>
    public void RegisterItem(Item item)
    {
        if (item != null && !string.IsNullOrEmpty(item.Id))
        {
            _items[item.Id] = item;
        }
    }
    
    /// <summary>
    /// Инициализирует стандартные предметы игры
    /// </summary>
    private void InitializeDefaultItems()
    {
        // Зелья
        RegisterItem(new Item
        {
            Id = "mushroom_heal",
            Name = "Гриб-целитель",
            Emoji = "🍄",
            Description = "Восстанавливает 20 HP",
            Type = ItemType.Consumable,
            HealthRestore = 20,
            MaxStack = 99
        });
        
        RegisterItem(new Item
        {
            Id = "herb_agility",
            Name = "Трава ловкости",
            Emoji = "🌿",
            Description = "Увеличивает ловкость на 3 на один бой",
            Type = ItemType.Consumable,
            AgilityBoost = 3,
            MaxStack = 99
        });
        
        // Оружие
        RegisterItem(new Item
        {
            Id = "shell_sword",
            Name = "Меч из ракушки",
            Emoji = "🗡️🐚",
            Description = "Острое оружие из панциря. +2 к силе",
            Type = ItemType.Weapon,
            StrengthBonus = 2
        });
        
        RegisterItem(new Item
        {
            Id = "iron_sword",
            Name = "Железный меч",
            Emoji = "🗡️",
            Description = "Надёжный меч. +4 к силе",
            Type = ItemType.Weapon,
            StrengthBonus = 4
        });
        
        // Броня
        RegisterItem(new Item
        {
            Id = "turtle_shell",
            Name = "Усиленный панцирь",
            Emoji = "🛡️",
            Description = "Укреплённый панцирь. +3 к защите",
            Type = ItemType.Armor,
            DefenseBonus = 3
        });
        
        RegisterItem(new Item
        {
            Id = "iron_armor",
            Name = "Железная броня",
            Emoji = "🛡️⚔️",
            Description = "Прочная броня. +5 к защите",
            Type = ItemType.Armor,
            DefenseBonus = 5
        });
        
        // Квестовые предметы
        RegisterItem(new Item
        {
            Id = "scroll_of_wisdom",
            Name = "Свиток Мудрости",
            Emoji = "📜",
            Description = "Древний артефакт, поддерживающий баланс мира",
            Type = ItemType.Quest
        });
    }
}

