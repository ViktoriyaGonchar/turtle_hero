namespace TurtleHero.Core.Models;

/// <summary>
/// Враг в игре
/// </summary>
public class Enemy
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Emoji { get; set; } = "🐍";
    
    public int MaxHealth { get; set; } = 30;
    private int _currentHealth;
    public int CurrentHealth
    {
        get => _currentHealth;
        set => _currentHealth = Math.Clamp(value, 0, MaxHealth);
    }
    
    public int Strength { get; set; } = 4;
    public int Agility { get; set; } = 2;
    public int Defense { get; set; } = 2;
    
    // Награды за победу
    public int ExperienceReward { get; set; } = 50;
    public List<ItemReward> ItemRewards { get; set; } = new();
    
    // Специальные способности
    public bool HasPoisonAttack { get; set; } = false; // Яд (урон каждый ход)
    public bool HasWebAttack { get; set; } = false;    // Паутина (снижает ловкость)
    
    // Временные эффекты
    public int PoisonDamage { get; set; } = 0; // Урон от яда
    public int AgilityDebuff { get; set; } = 0; // Снижение ловкости
    
    public Enemy()
    {
        CurrentHealth = MaxHealth;
    }
    
    public bool IsAlive => CurrentHealth > 0;
    
    public void TakeDamage(int damage)
    {
        if (damage <= 0) return;
        var actualDamage = Math.Max(1, damage - Defense);
        CurrentHealth = Math.Max(0, CurrentHealth - actualDamage);
    }
    
    public void FullRestore()
    {
        CurrentHealth = MaxHealth;
        PoisonDamage = 0;
        AgilityDebuff = 0;
    }
}

/// <summary>
/// Награда предметом за победу над врагом
/// </summary>
public class ItemReward
{
    public string ItemId { get; set; } = string.Empty;
    public int Quantity { get; set; } = 1;
    public int DropChance { get; set; } = 100; // Процент шанса выпадения (0-100)
}

