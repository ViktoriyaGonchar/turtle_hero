using TurtleHero.Core.Models;

namespace TurtleHero.Core.Data;

/// <summary>
/// Репозиторий врагов игры
/// </summary>
public class EnemyRepository
{
    private readonly Dictionary<string, Enemy> _enemies = new();
    
    public EnemyRepository()
    {
        InitializeDefaultEnemies();
    }
    
    /// <summary>
    /// Получает врага по ID
    /// </summary>
    public Enemy? GetEnemy(string enemyId)
    {
        return _enemies.TryGetValue(enemyId, out var enemy) ? enemy : null;
    }
    
    /// <summary>
    /// Создаёт копию врага (для боя)
    /// </summary>
    public Enemy? CreateEnemy(string enemyId)
    {
        if (!_enemies.TryGetValue(enemyId, out var template))
            return null;
        
        // Создаём копию врага для боя
        return new Enemy
        {
            Id = template.Id,
            Name = template.Name,
            Emoji = template.Emoji,
            MaxHealth = template.MaxHealth,
            CurrentHealth = template.MaxHealth,
            Strength = template.Strength,
            Agility = template.Agility,
            Defense = template.Defense,
            ExperienceReward = template.ExperienceReward,
            ItemRewards = new List<ItemReward>(template.ItemRewards),
            HasPoisonAttack = template.HasPoisonAttack,
            HasWebAttack = template.HasWebAttack
        };
    }
    
    /// <summary>
    /// Регистрирует врага
    /// </summary>
    public void RegisterEnemy(Enemy enemy)
    {
        if (enemy != null && !string.IsNullOrEmpty(enemy.Id))
        {
            _enemies[enemy.Id] = enemy;
        }
    }
    
    /// <summary>
    /// Инициализирует стандартных врагов игры
    /// </summary>
    private void InitializeDefaultEnemies()
    {
        // Змея-страж
        RegisterEnemy(new Enemy
        {
            Id = "snake_guard",
            Name = "Змея-страж",
            Emoji = "🐍",
            MaxHealth = 30,
            Strength = 4,
            Agility = 2,
            Defense = 2,
            ExperienceReward = 50,
            ItemRewards = new List<ItemReward>
            {
                new() { ItemId = "mushroom_heal", Quantity = 1, DropChance = 50 }
            },
            HasPoisonAttack = true
        });
        
        // Скорпион-наёмник
        RegisterEnemy(new Enemy
        {
            Id = "scorpion_mercenary",
            Name = "Скорпион-наёмник",
            Emoji = "🦂",
            MaxHealth = 45,
            Strength = 6,
            Agility = 3,
            Defense = 3,
            ExperienceReward = 80,
            ItemRewards = new List<ItemReward>
            {
                new() { ItemId = "mushroom_heal", Quantity = 2, DropChance = 60 },
                new() { ItemId = "shell_sword", Quantity = 1, DropChance = 20 }
            },
            HasPoisonAttack = true
        });
        
        // Паук-иллюзионист
        RegisterEnemy(new Enemy
        {
            Id = "spider_illusionist",
            Name = "Паук-иллюзионист",
            Emoji = "🕷️",
            MaxHealth = 35,
            Strength = 3,
            Agility = 5,
            Defense = 2,
            ExperienceReward = 70,
            ItemRewards = new List<ItemReward>
            {
                new() { ItemId = "herb_agility", Quantity = 1, DropChance = 40 }
            },
            HasWebAttack = true
        });
        
        // Ящер-предатель
        RegisterEnemy(new Enemy
        {
            Id = "lizard_traitor",
            Name = "Ящер-предатель",
            Emoji = "🦎",
            MaxHealth = 50,
            Strength = 5,
            Agility = 4,
            Defense = 4,
            ExperienceReward = 100,
            ItemRewards = new List<ItemReward>
            {
                new() { ItemId = "mushroom_heal", Quantity = 3, DropChance = 70 },
                new() { ItemId = "turtle_shell", Quantity = 1, DropChance = 30 }
            }
        });
        
        // Змеиный Тиран (финальный босс)
        RegisterEnemy(new Enemy
        {
            Id = "snake_tyrant",
            Name = "Змеиный Тиран",
            Emoji = "🐍👑",
            MaxHealth = 150,
            Strength = 12,
            Agility = 6,
            Defense = 8,
            ExperienceReward = 500,
            ItemRewards = new List<ItemReward>
            {
                new() { ItemId = "scroll_of_wisdom", Quantity = 1, DropChance = 100 },
                new() { ItemId = "iron_sword", Quantity = 1, DropChance = 50 },
                new() { ItemId = "iron_armor", Quantity = 1, DropChance = 50 }
            },
            HasPoisonAttack = true
        });
    }
}

