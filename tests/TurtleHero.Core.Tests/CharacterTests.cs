using FluentAssertions;
using TurtleHero.Core.Models;
using Xunit;

namespace TurtleHero.Core.Tests;

public class CharacterTests
{
    [Fact]
    public void Character_Level_Should_Not_Be_Below_1()
    {
        // Arrange
        var character = new Character();
        character.Level = 0;
        
        // Act
        character.LevelUp();
        
        // Assert
        character.Level.Should().BeGreaterThanOrEqualTo(1);
    }
    
    [Fact]
    public void Character_Health_Should_Not_Exceed_MaxHealth()
    {
        // Arrange
        var character = new Character { MaxHealth = 50, CurrentHealth = 50 };
        
        // Act
        character.Heal(100);
        
        // Assert
        character.CurrentHealth.Should().Be(50);
        character.CurrentHealth.Should().BeLessThanOrEqualTo(character.MaxHealth);
    }
    
    [Fact]
    public void Character_Health_Should_Not_Be_Negative()
    {
        // Arrange
        var character = new Character { CurrentHealth = 10 };
        
        // Act
        character.TakeDamage(1000);
        
        // Assert
        character.CurrentHealth.Should().BeGreaterThanOrEqualTo(1); // Панцирь спасает
    }
    
    [Fact]
    public void Character_Should_Gain_Experience()
    {
        // Arrange
        var character = new Character { Level = 1, Experience = 0 };
        
        // Act
        var leveledUp = character.AddExperience(100);
        
        // Assert
        // После levelup опыт может быть 0 (ровно достаточно опыта) или больше (если был излишек)
        character.Experience.Should().BeGreaterThanOrEqualTo(0);
        leveledUp.Should().BeTrue();
        character.Level.Should().Be(2);
    }
    
    [Fact]
    public void Character_LevelUp_Should_Increase_MaxHealth()
    {
        // Arrange
        var character = new Character { Level = 1, MaxHealth = 50 };
        var oldMaxHealth = character.MaxHealth;
        
        // Act
        character.LevelUp();
        
        // Assert
        character.MaxHealth.Should().Be(oldMaxHealth + 5);
        character.CurrentHealth.Should().Be(character.MaxHealth); // Полное восстановление
    }
    
    [Fact]
    public void Character_LevelUp_Should_Increase_Stats()
    {
        // Arrange
        var character = new Character { Level = 1, Strength = 5, Agility = 3, Defense = 4 };
        var oldStrength = character.Strength;
        var oldAgility = character.Agility;
        var oldDefense = character.Defense;
        
        // Act
        character.LevelUp();
        
        // Assert
        // Одна из характеристик должна увеличиться
        var strengthIncreased = character.Strength > oldStrength;
        var agilityIncreased = character.Agility > oldAgility;
        var defenseIncreased = character.Defense > oldDefense;
        
        (strengthIncreased || agilityIncreased || defenseIncreased).Should().BeTrue();
    }
    
    [Fact]
    public void Character_EffectiveStrength_Should_Include_Weapon_Bonus()
    {
        // Arrange
        var character = new Character { Strength = 5 };
        var weapon = new Item { StrengthBonus = 3 };
        character.EquippedWeapon = weapon;
        
        // Act
        var effectiveStrength = character.EffectiveStrength;
        
        // Assert
        effectiveStrength.Should().Be(8); // 5 + 3
    }
    
    [Fact]
    public void Character_EffectiveDefense_Should_Include_Armor_Bonus()
    {
        // Arrange
        var character = new Character { Defense = 4 };
        var armor = new Item { DefenseBonus = 3 };
        character.EquippedArmor = armor;
        
        // Act
        var effectiveDefense = character.EffectiveDefense;
        
        // Assert
        effectiveDefense.Should().Be(7); // 4 + 3
    }
    
    [Fact]
    public void Character_Should_Be_Alive_When_Health_Greater_Than_Zero()
    {
        // Arrange & Act
        var character = new Character { CurrentHealth = 1 };
        
        // Assert
        character.IsAlive.Should().BeTrue();
    }
    
    [Fact]
    public void Character_Should_Not_Be_Alive_When_Health_Is_Zero()
    {
        // Arrange & Act
        var character = new Character { CurrentHealth = 0 };
        
        // Assert
        character.IsAlive.Should().BeFalse();
    }
    
    [Fact]
    public void Character_Negative_Experience_Should_Not_LevelUp()
    {
        // Arrange
        var character = new Character { Level = 1, Experience = 0 };
        
        // Act
        var leveledUp = character.AddExperience(-50);
        
        // Assert
        leveledUp.Should().BeFalse();
        character.Level.Should().Be(1);
    }
}

