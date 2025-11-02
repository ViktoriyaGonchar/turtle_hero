using FluentAssertions;
using TurtleHero.Core.Storage;
using Xunit;

namespace TurtleHero.Integration;

public class ErrorHandlingTests
{
    [Fact]
    public void Load_NonExistent_Save_Should_Not_Crash()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var saveManager = new SaveGameManager(tempDir);
        
        // Act
        var result = saveManager.LoadGame();
        
        // Assert
        result.Should().BeNull(); // Не крашится, возвращает null
        
        // Cleanup
        Directory.Delete(tempDir, true);
    }
    
    [Fact]
    public void Load_Corrupted_Save_Should_Not_Crash()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        var saveManager = new SaveGameManager(tempDir);
        File.WriteAllText(saveManager.SaveFilePath, "corrupted json data {{invalid");
        
        // Act
        var result = saveManager.LoadGame();
        
        // Assert
        result.Should().BeNull(); // Не крашится, возвращает null
        
        // Cleanup
        Directory.Delete(tempDir, true);
    }
    
    [Fact]
    public void Load_Empty_File_Should_Not_Crash()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        var saveManager = new SaveGameManager(tempDir);
        File.WriteAllText(saveManager.SaveFilePath, "");
        
        // Act
        var result = saveManager.LoadGame();
        
        // Assert
        result.Should().BeNull();
        
        // Cleanup
        Directory.Delete(tempDir, true);
    }
    
    [Fact]
    public void Scenario_Loader_Should_Handle_Invalid_JSON()
    {
        // Arrange
        var loader = new ScenarioLoader();
        
        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => 
            loader.LoadFromJson("invalid json"));
        
        exception.Should().NotBeNull();
    }
    
    [Fact]
    public void Scenario_Loader_Should_Handle_Missing_File()
    {
        // Arrange
        var loader = new ScenarioLoader();
        var nonExistentPath = Path.Combine(Path.GetTempPath(), "nonexistent_scenario.json");
        
        // Act & Assert
        var exception = Assert.Throws<FileNotFoundException>(() => 
            loader.LoadFromFile(nonExistentPath));
        
        exception.Should().NotBeNull();
    }
    
    [Fact]
    public void Character_Should_Handle_Negative_Damage()
    {
        // Arrange
        var character = new Core.Models.Character { CurrentHealth = 50 };
        var healthBefore = character.CurrentHealth;
        
        // Act
        character.TakeDamage(-10); // Отрицательный урон
        
        // Assert
        character.CurrentHealth.Should().Be(healthBefore); // Здоровье не изменилось
    }
    
    [Fact]
    public void Inventory_Should_Handle_Invalid_Operations()
    {
        // Arrange
        var inventory = new Core.Models.Inventory();
        
        // Act & Assert
        inventory.AddItem(null!, 1).Should().BeFalse(); // null предмет
        inventory.AddItem(new Core.Models.Item { Id = "test" }, 0).Should().BeFalse(); // нулевое количество
        inventory.RemoveItem("nonexistent", 1).Should().BeFalse(); // несуществующий предмет
    }
}

