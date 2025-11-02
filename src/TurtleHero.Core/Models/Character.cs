namespace TurtleHero.Core.Models;

/// <summary>
/// Представляет игрового персонажа - черепашку-героя
/// </summary>
public class Character
{
    public string Name { get; set; } = "Тортилла 🐢";
    public string Emoji { get; set; } = "🐢";
    
    // Основные характеристики
    public int Level { get; set; } = 1;
    public int Experience { get; set; } = 0;
    public int ExperienceToNextLevel => Level * 100; // Опыт для следующего уровня
    
    // Боевые характеристики
    public int MaxHealth { get; set; } = 50;
    private int _currentHealth;
    public int CurrentHealth
    {
        get => _currentHealth;
        set => _currentHealth = Math.Clamp(value, 0, MaxHealth);
    }
    
    public int Strength { get; set; } = 5; // Сила (влияет на урон)
    public int Agility { get; set; } = 3;  // Ловкость (влияет на инициативу)
    public int Defense { get; set; } = 4;  // Защита (снижает получаемый урон)
    
    // Экипировка
    public Item? EquippedWeapon { get; set; }
    public Item? EquippedArmor { get; set; }
    
    // Временные модификаторы (для боя)
    public int TemporaryDefenseBonus { get; set; } = 0;
    
    /// <summary>
    /// Текущая сила с учётом экипировки
    /// </summary>
    public int EffectiveStrength => Strength + (EquippedWeapon?.StrengthBonus ?? 0);
    
    /// <summary>
    /// Текущая защита с учётом экипировки и временных модификаторов
    /// </summary>
    public int EffectiveDefense => Defense + (EquippedArmor?.DefenseBonus ?? 0) + TemporaryDefenseBonus;
    
    /// <summary>
    /// Текущее здоровье в процентах
    /// </summary>
    public double HealthPercentage => MaxHealth > 0 ? (double)CurrentHealth / MaxHealth * 100 : 0;
    
    public Character()
    {
        CurrentHealth = MaxHealth;
    }
    
    /// <summary>
    /// Добавляет опыт и проверяет повышение уровня
    /// </summary>
    public bool AddExperience(int xp)
    {
        if (xp <= 0) return false;
        
        Experience += xp;
        bool leveledUp = false;
        
        // Проверяем, достаточно ли опыта для повышения уровня
        while (Experience >= ExperienceToNextLevel && Level >= 1)
        {
            LevelUp();
            leveledUp = true;
        }
        
        return leveledUp;
    }
    
    /// <summary>
    /// Повышает уровень персонажа
    /// </summary>
    public void LevelUp()
    {
        if (Level < 1) Level = 1; // Защита от уровня < 1
        
        // Вычисляем необходимый опыт ДО увеличения уровня
        var requiredExp = Level * 100;
        Level++;
        Experience -= requiredExp;
        
        // Увеличиваем характеристики при повышении уровня
        MaxHealth += 5;
        CurrentHealth = MaxHealth; // Полное восстановление при повышении уровня
        
        // Случайное увеличение одной из характеристик
        var random = new Random();
        switch (random.Next(0, 3))
        {
            case 0:
                Strength++;
                break;
            case 1:
                Agility++;
                break;
            case 2:
                Defense++;
                break;
        }
    }
    
    /// <summary>
    /// Восстанавливает здоровье
    /// </summary>
    public void Heal(int amount)
    {
        CurrentHealth = Math.Min(CurrentHealth + amount, MaxHealth);
    }
    
    /// <summary>
    /// Наносит урон персонажу
    /// </summary>
    public void TakeDamage(int damage, bool allowDeath = false)
    {
        if (damage <= 0) return;
        
        // Вычисляем фактический урон с учётом защиты
        var actualDamage = Math.Max(1, damage - EffectiveDefense);
        CurrentHealth -= actualDamage;
        
        // Панцирь спасает - минимальное HP = 1 (если не разрешена смерть)
        if (!allowDeath && CurrentHealth < 1)
        {
            CurrentHealth = 1;
        }
        else if (CurrentHealth < 0)
        {
            CurrentHealth = 0;
        }
    }
    
    /// <summary>
    /// Проверяет, жив ли персонаж
    /// </summary>
    public bool IsAlive => CurrentHealth > 0;
    
    /// <summary>
    /// Полностью восстанавливает здоровье
    /// </summary>
    public void FullRestore()
    {
        CurrentHealth = MaxHealth;
        TemporaryDefenseBonus = 0;
    }
}

