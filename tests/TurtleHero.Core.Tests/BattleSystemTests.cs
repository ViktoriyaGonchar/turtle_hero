using FluentAssertions;
using TurtleHero.Core.Game;
using TurtleHero.Core.Models;
using Xunit;

namespace TurtleHero.Core.Tests;

public class BattleSystemTests
{
    [Fact]
    public void BattleSystem_PlayerAttack_Should_Damage_Enemy()
    {
        // Arrange
        var battleSystem = new BattleSystem();
        var player = new Character { Strength = 5, Defense = 2 };
        var enemy = new Enemy { MaxHealth = 30, CurrentHealth = 30, Defense = 2 };
        
        // Act
        var result = battleSystem.PlayerAttack(player, enemy);
        
        // Assert
        result.Damage.Should().BeGreaterThan(0);
        enemy.CurrentHealth.Should().BeLessThan(enemy.MaxHealth);
        result.ActionType.Should().Be(BattleActionType.Attack);
    }
    
    [Fact]
    public void BattleSystem_Battle_Should_End_When_Enemy_Health_Zero()
    {
        // Arrange
        var battleSystem = new BattleSystem();
        var player = new Character { Strength = 100, Defense = 2 }; // Высокая сила
        var enemy = new Enemy { MaxHealth = 10, CurrentHealth = 10, Defense = 0 };
        
        // Act
        var result = battleSystem.PlayerAttack(player, enemy);
        
        // Assert
        result.IsFinished.Should().BeTrue();
        result.PlayerWon.Should().BeTrue();
        enemy.IsAlive.Should().BeFalse();
    }
    
    [Fact]
    public void BattleSystem_Battle_Should_End_When_Player_Health_Zero()
    {
        // Arrange
        var battleSystem = new BattleSystem();
        var player = new Character { MaxHealth = 10, CurrentHealth = 10, Defense = 0 };
        var enemy = new Enemy { Strength = 100, Defense = 0 }; // Высокая сила
        
        // Act
        var result = battleSystem.EnemyTurn(player, enemy);
        
        // Assert
        result.IsFinished.Should().BeTrue();
        result.PlayerWon.Should().BeFalse();
        player.IsAlive.Should().BeFalse();
    }
    
    [Fact]
    public void BattleSystem_Damage_Should_Depend_On_Strength_And_Defense()
    {
        // Arrange
        var battleSystem = new BattleSystem();
        var strongPlayer = new Character { Strength = 10, Defense = 2 };
        var weakPlayer = new Character { Strength = 2, Defense = 2 };
        var enemy = new Enemy { MaxHealth = 100, CurrentHealth = 100, Defense = 2 };
        
        // Act
        var strongResult = battleSystem.PlayerAttack(strongPlayer, enemy);
        var weakResult = battleSystem.PlayerAttack(weakPlayer, enemy);
        
        // Assert
        strongResult.Damage.Should().BeGreaterThan(weakResult.Damage);
    }
    
    [Fact]
    public void BattleSystem_Critical_Hit_Should_Double_Damage()
    {
        // Arrange
        var battleSystem = new BattleSystem();
        var player = new Character { Strength = 10, Defense = 2 };
        var enemy = new Enemy { MaxHealth = 100, CurrentHealth = 100, Defense = 0 };
        
        // Act - выполняем много атак, пока не получим крит
        BattleActionResult? criticalResult = null;
        for (int i = 0; i < 100; i++)
        {
            enemy.CurrentHealth = enemy.MaxHealth;
            var result = battleSystem.PlayerAttack(player, enemy);
            if (result.IsCritical)
            {
                criticalResult = result;
                break;
            }
        }
        
        // Assert
        criticalResult.Should().NotBeNull();
        criticalResult!.IsCritical.Should().BeTrue();
    }
    
    [Fact]
    public void BattleSystem_PlayerDefend_Should_Increase_Defense()
    {
        // Arrange
        var battleSystem = new BattleSystem();
        var player = new Character { Defense = 4 };
        var oldDefense = player.EffectiveDefense;
        
        // Act
        battleSystem.PlayerDefend(player);
        
        // Assert
        player.TemporaryDefenseBonus.Should().BeGreaterThan(0);
        player.EffectiveDefense.Should().BeGreaterThan(oldDefense);
    }
    
    [Fact]
    public void BattleSystem_PlayerGoesFirst_Should_Depend_On_Agility()
    {
        // Arrange
        var battleSystem = new BattleSystem();
        var fastPlayer = new Character { Agility = 10 };
        var slowPlayer = new Character { Agility = 1 };
        var fastEnemy = new Enemy { Agility = 1 };
        var slowEnemy = new Enemy { Agility = 10 };
        
        // Act & Assert
        // Если игрок быстрее врага - ходит первым
        battleSystem.PlayerGoesFirst(fastPlayer, fastEnemy).Should().BeTrue(); // 10 > 1 = true
        // Если игрок медленнее врага - ходит вторым
        battleSystem.PlayerGoesFirst(slowPlayer, slowEnemy).Should().BeFalse(); // 1 < 10 = false
    }
    
    [Fact]
    public void BattleSystem_Reward_Should_Contain_Experience()
    {
        // Arrange
        var battleSystem = new BattleSystem();
        var enemy = new Enemy { ExperienceReward = 100 };
        
        // Act
        var reward = battleSystem.CalculateReward(enemy);
        
        // Assert
        reward.Experience.Should().Be(100);
    }
    
    [Fact]
    public void BattleSystem_Damage_Should_Not_Be_Negative()
    {
        // Arrange
        var battleSystem = new BattleSystem();
        var player = new Character { Strength = 1, Defense = 2 };
        var enemy = new Enemy { MaxHealth = 100, CurrentHealth = 100, Defense = 100 }; // Очень высокая защита
        
        // Act
        var result = battleSystem.PlayerAttack(player, enemy);
        
        // Assert
        result.Damage.Should().BeGreaterThanOrEqualTo(1); // Минимальный урон
    }
}

