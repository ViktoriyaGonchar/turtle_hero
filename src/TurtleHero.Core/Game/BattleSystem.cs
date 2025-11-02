using TurtleHero.Core.Models;

namespace TurtleHero.Core.Game;

/// <summary>
/// Результат действия в бою
/// </summary>
public enum BattleActionType
{
    Attack,
    Defend,
    UseItem,
    Run
}

/// <summary>
/// Результат одного действия в бою
/// </summary>
public class BattleActionResult
{
    public BattleActionType ActionType { get; set; }
    public string Message { get; set; } = string.Empty;
    public int Damage { get; set; } = 0;
    public bool IsCritical { get; set; } = false;
    public bool IsFinished { get; set; } = false;
    public bool PlayerWon { get; set; } = false;
}

/// <summary>
/// Система пошагового боя
/// </summary>
public class BattleSystem
{
    private readonly Random _random = new();
    
    /// <summary>
    /// Выполняет атаку персонажа по врагу
    /// </summary>
    public BattleActionResult PlayerAttack(Character player, Enemy enemy)
    {
        if (!player.IsAlive || !enemy.IsAlive)
        {
            return new BattleActionResult
            {
                ActionType = BattleActionType.Attack,
                Message = "Бой уже завершён!",
                IsFinished = true
            };
        }
        
        // Расчёт урона
        var baseDamage = player.EffectiveStrength;
        var randomVariation = _random.Next(-2, 3); // ±2 случайное отклонение
        var damage = Math.Max(1, baseDamage - enemy.Defense + randomVariation);
        
        // Критический удар (10% шанс)
        bool isCritical = _random.Next(0, 100) < 10;
        if (isCritical)
        {
            damage *= 2;
        }
        
        enemy.TakeDamage(damage);
        
        var result = new BattleActionResult
        {
            ActionType = BattleActionType.Attack,
            Damage = damage,
            IsCritical = isCritical,
            Message = isCritical 
                ? $"💥 Критический удар! {player.Emoji} наносит {damage} урона {enemy.Emoji}!"
                : $"⚔️ {player.Emoji} атакует {enemy.Emoji} и наносит {damage} урона!"
        };
        
        // Проверка завершения боя
        if (!enemy.IsAlive)
        {
            result.IsFinished = true;
            result.PlayerWon = true;
            result.Message += $"\n🎉 Победа! {enemy.Emoji} повержен!";
        }
        
        return result;
    }
    
    /// <summary>
    /// Выполняет защиту персонажа (увеличивает защиту на 50%)
    /// </summary>
    public BattleActionResult PlayerDefend(Character player)
    {
        if (!player.IsAlive)
        {
            return new BattleActionResult
            {
                ActionType = BattleActionType.Defend,
                Message = "Персонаж не может защищаться!",
                IsFinished = true
            };
        }
        
        player.TemporaryDefenseBonus = (int)(player.EffectiveDefense * 0.5);
        
        return new BattleActionResult
        {
            ActionType = BattleActionType.Defend,
            Message = $"🛡️ {player.Emoji} принимает защитную стойку! Защита увеличена!"
        };
    }
    
    /// <summary>
    /// Выполняет ход врага
    /// </summary>
    public BattleActionResult EnemyTurn(Character player, Enemy enemy)
    {
        if (!player.IsAlive || !enemy.IsAlive)
        {
            return new BattleActionResult
            {
                ActionType = BattleActionType.Attack,
                Message = "Бой уже завершён!",
                IsFinished = true
            };
        }
        
        // Урон от яда (если есть)
        if (enemy.HasPoisonAttack && enemy.PoisonDamage > 0)
        {
            player.TakeDamage(enemy.PoisonDamage, allowDeath: true);
            var poisonResult = new BattleActionResult
            {
                ActionType = BattleActionType.Attack,
                Damage = enemy.PoisonDamage,
                Message = $"☠️ Яд наносит {enemy.PoisonDamage} урона {player.Emoji}!"
            };
            
            if (!player.IsAlive)
            {
                poisonResult.IsFinished = true;
                poisonResult.PlayerWon = false;
                poisonResult.Message += $"\n💀 {player.Emoji} пал в бою...";
            }
            
            return poisonResult;
        }
        
        // Обычная атака врага
        var baseDamage = enemy.Strength;
        var randomVariation = _random.Next(-1, 2);
        var damage = Math.Max(1, baseDamage - player.EffectiveDefense + randomVariation);
        
        bool isCritical = _random.Next(0, 100) < 5; // Враг реже критикует (5%)
        if (isCritical)
        {
            damage *= 2;
        }
        
        player.TakeDamage(damage, allowDeath: true);
        
        var result = new BattleActionResult
        {
            ActionType = BattleActionType.Attack,
            Damage = damage,
            IsCritical = isCritical,
            Message = isCritical
                ? $"💥 Критический удар! {enemy.Emoji} наносит {damage} урона {player.Emoji}!"
                : $"⚔️ {enemy.Emoji} атакует {player.Emoji} и наносит {damage} урона!"
        };
        
        // Применение специальных способностей
        if (enemy.HasWebAttack && _random.Next(0, 100) < 30) // 30% шанс
        {
            enemy.AgilityDebuff = 2;
            result.Message += $"\n🕸️ Паутина замедляет {player.Emoji}! Ловкость снижена!";
        }
        
        if (enemy.HasPoisonAttack && _random.Next(0, 100) < 25) // 25% шанс отравить
        {
            enemy.PoisonDamage = 3; // 3 урона каждый ход
            result.Message += $"\n☠️ {player.Emoji} отравлен!";
        }
        
        // Проверка завершения боя
        if (!player.IsAlive)
        {
            result.IsFinished = true;
            result.PlayerWon = false;
            result.Message += $"\n💀 {player.Emoji} пал в бою...";
        }
        
        return result;
    }
    
    /// <summary>
    /// Определяет, кто ходит первым (на основе ловкости)
    /// </summary>
    public bool PlayerGoesFirst(Character player, Enemy enemy)
    {
        if (player.Agility > enemy.Agility) return true;
        if (player.Agility < enemy.Agility) return false;
        
        // При равенстве - случайный выбор (50% шанс)
        return _random.Next(0, 2) == 1;
    }
    
    /// <summary>
    /// Вычисляет награду за победу
    /// </summary>
    public BattleReward CalculateReward(Enemy enemy)
    {
        var reward = new BattleReward
        {
            Experience = enemy.ExperienceReward,
            Items = new List<(Item, int)>()
        };
        
        // Предметы награды
        foreach (var itemReward in enemy.ItemRewards)
        {
            if (_random.Next(0, 100) < itemReward.DropChance)
            {
                // Предмет будет добавлен позже, когда будет доступен репозиторий предметов
                reward.Items.Add((null!, itemReward.Quantity));
            }
        }
        
        return reward;
    }
}

/// <summary>
/// Награда за победу в бою
/// </summary>
public class BattleReward
{
    public int Experience { get; set; }
    public List<(Item Item, int Quantity)> Items { get; set; } = new();
}

