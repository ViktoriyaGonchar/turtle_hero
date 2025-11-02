using FluentAssertions;
using TurtleHero.Core.Game;
using TurtleHero.Core.Storage;
using Xunit;

namespace TurtleHero.Core.Tests;

public class SaveGameManagerTests
{
    [Fact]
    public void SaveGameManager_Save_And_Load_Should_Preserve_Data()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var saveManager = new SaveGameManager(tempDir);
        var gameState = new GameState
        {
            Player = new Models.Character
            {
                Name = "Тестовая черепашка",
                Level = 5,
                Experience = 250,
                CurrentHealth = 45,
                MaxHealth = 75,
                Strength = 10,
                Agility = 7,
                Defense = 8
            },
            CurrentLocation = "test_location",
            GameFlags = { ["test_flag"] = true }
        };
        gameState.Inventory.AddItem(new Models.Item { Id = "test_item", Name = "Тестовый предмет" }, 3);
        
        // Act
        var saveResult = saveManager.SaveGame(gameState);
        var loadedState = saveManager.LoadGame();
        
        // Assert
        saveResult.Should().BeTrue();
        loadedState.Should().NotBeNull();
        loadedState!.Player.Name.Should().Be("Тестовая черепашка");
        loadedState.Player.Level.Should().Be(5);
        loadedState.Player.Experience.Should().Be(250);
        loadedState.Player.CurrentHealth.Should().Be(45);
        loadedState.Player.MaxHealth.Should().Be(75);
        loadedState.CurrentLocation.Should().Be("test_location");
        loadedState.HasFlag("test_flag").Should().BeTrue();
        loadedState.Inventory.HasItem("test_item", 3).Should().BeTrue();
        
        // Cleanup
        Directory.Delete(tempDir, true);
    }
    
    [Fact]
    public void SaveGameManager_Load_NonExistent_Save_Should_Return_Null()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var saveManager = new SaveGameManager(tempDir);
        
        // Act
        var loadedState = saveManager.LoadGame();
        
        // Assert
        loadedState.Should().BeNull();
        saveManager.SaveExists().Should().BeFalse();
        
        // Cleanup
        Directory.Delete(tempDir, true);
    }
    
    [Fact]
    public void SaveGameManager_SaveExists_Should_Return_True_After_Save()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var saveManager = new SaveGameManager(tempDir);
        var gameState = new GameState();
        
        // Act
        saveManager.SaveGame(gameState);
        
        // Assert
        saveManager.SaveExists().Should().BeTrue();
        
        // Cleanup
        Directory.Delete(tempDir, true);
    }
    
    [Fact]
    public void SaveGameManager_DeleteSave_Should_Remove_Save_File()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var saveManager = new SaveGameManager(tempDir);
        var gameState = new GameState();
        saveManager.SaveGame(gameState);
        
        // Act
        var deleteResult = saveManager.DeleteSave();
        
        // Assert
        deleteResult.Should().BeTrue();
        saveManager.SaveExists().Should().BeFalse();
        
        // Cleanup
        Directory.Delete(tempDir, true);
    }
    
    [Fact]
    public void SaveGameManager_Load_Invalid_Json_Should_Return_Null()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        var saveManager = new SaveGameManager(tempDir);
        File.WriteAllText(saveManager.SaveFilePath, "invalid json {");
        
        // Act
        var loadedState = saveManager.LoadGame();
        
        // Assert
        loadedState.Should().BeNull();
        
        // Cleanup
        Directory.Delete(tempDir, true);
    }
    
    [Fact]
    public void SaveGameManager_Load_Should_Validate_And_Fix_Invalid_Data()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        var saveManager = new SaveGameManager(tempDir);
        
        // Создаём JSON с невалидными данными (отрицательный уровень, здоровье выше максимума)
        var invalidJson = @"{
            ""player"": {
                ""level"": -1,
                ""maxHealth"": 50,
                ""currentHealth"": 100,
                ""strength"": 5,
                ""agility"": 3,
                ""defense"": 4
            },
            ""inventory"": {},
            ""currentLocation"": ""test"",
            ""gameFlags"": {}
        }";
        
        File.WriteAllText(saveManager.SaveFilePath, invalidJson);
        
        // Act
        var loadedState = saveManager.LoadGame();
        
        // Assert
        loadedState.Should().NotBeNull();
        loadedState!.Player.Level.Should().BeGreaterOrEqualTo(1);
        loadedState.Player.CurrentHealth.Should().BeLessOrEqualTo(loadedState.Player.MaxHealth);
        
        // Cleanup
        Directory.Delete(tempDir, true);
    }
}

