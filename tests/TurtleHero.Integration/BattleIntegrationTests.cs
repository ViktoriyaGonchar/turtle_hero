using FluentAssertions;
using TurtleHero.Core.Data;
using TurtleHero.Core.Game;
using TurtleHero.Core.Models;
using Xunit;

namespace TurtleHero.Integration;

public class BattleIntegrationTests
{
    [Fact]
    public void Battle_Should_Give_Experience_And_Level_Up()
    {
        // Arrange
        var battleSystem = new BattleSystem();
        var player = new Character { Level = 1, Experience = 0, Strength = 10, MaxHealth = 50, CurrentHealth = 50 };
        var enemy = new Enemy
        {
            MaxHealth = 20,
            CurrentHealth = 20,
            Defense = 0,
            ExperienceReward = 100
        };
        
        // Act - победить врага
        while (enemy.IsAlive && player.IsAlive)
        {
            battleSystem.PlayerAttack(player, enemy);
            if (enemy.IsAlive)
            {
                battleSystem.EnemyTurn(player, enemy);
            }
        }
        
        // Применить награду
        var reward = battleSystem.CalculateReward(enemy);
        var leveledUp = player.AddExperience(reward.Experience);
        
        // Assert
        enemy.IsAlive.Should().BeFalse();
        player.IsAlive.Should().BeTrue();
        leveledUp.Should().BeTrue();
        player.Level.Should().BeGreaterThan(1);
    }
    
    [Fact]
    public void Battle_After_LevelUp_Should_Have_New_Stats()
    {
        // Arrange
        var battleSystem = new BattleSystem();
        var player = new Character { Level = 1, Strength = 5, MaxHealth = 50 };
        var oldStrength = player.Strength;
        var oldMaxHealth = player.MaxHealth;
        
        // Act
        player.AddExperience(100); // Достаточно для повышения уровня
        
        // Assert
        player.Level.Should().Be(2);
        player.MaxHealth.Should().BeGreaterThan(oldMaxHealth);
        // Одна из характеристик должна увеличиться
        (player.Strength > oldStrength || player.Agility > 0 || player.Defense > 0).Should().BeTrue();
    }
    
    [Fact]
    public void Use_Item_In_Battle_Should_Restore_Health()
    {
        // Arrange
        var battleSystem = new BattleSystem();
        var player = new Character { MaxHealth = 50, CurrentHealth = 20, Strength = 10 };
        var enemy = new Enemy { MaxHealth = 100, CurrentHealth = 100, Defense = 0 };
        var healingItem = new Item { Id = "heal", HealthRestore = 20 };
        
        // Act - получаем урон, затем лечимся
        battleSystem.EnemyTurn(player, enemy);
        var healthBeforeHeal = player.CurrentHealth;
        player.Heal(healingItem.HealthRestore);
        
        // Assert
        player.CurrentHealth.Should().BeGreaterThan(healthBeforeHeal);
        player.CurrentHealth.Should().BeLessOrEqualTo(player.MaxHealth);
    }
}

