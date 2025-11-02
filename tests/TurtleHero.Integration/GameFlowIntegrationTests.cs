using FluentAssertions;
using TurtleHero.Core.Data;
using TurtleHero.Core.Game;
using TurtleHero.Core.Game.Dialogue;
using TurtleHero.Core.Models;
using TurtleHero.Core.Storage;
using Xunit;

namespace TurtleHero.Integration;

public class GameFlowIntegrationTests
{
    [Fact]
    public void Full_Game_Cycle_Create_Battle_LevelUp_Save_Load()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var saveManager = new SaveGameManager(tempDir);
        var gameState = new GameState();
        var battleSystem = new BattleSystem();
        var itemRepository = new ItemRepository();
        
        // Act 1: Создание персонажа и добавление предметов
        gameState.Inventory.AddItem(itemRepository.GetItem("mushroom_heal")!, 3);
        
        // Act 2: Бой
        var enemy = new Enemy
        {
            MaxHealth = 30,
            CurrentHealth = 30,
            Defense = 2,
            ExperienceReward = 100
        };
        
        while (enemy.IsAlive && gameState.Player.IsAlive)
        {
            battleSystem.PlayerAttack(gameState.Player, enemy);
            if (enemy.IsAlive)
            {
                battleSystem.EnemyTurn(gameState.Player, enemy);
            }
        }
        
        // Act 3: Получение награды и повышение уровня
        var reward = battleSystem.CalculateReward(enemy);
        gameState.Player.AddExperience(reward.Experience);
        
        // Act 4: Использование предмета
        var healItem = itemRepository.GetItem("mushroom_heal");
        if (healItem != null && gameState.Inventory.HasItem("mushroom_heal"))
        {
            gameState.Player.Heal(healItem.HealthRestore);
            gameState.Inventory.RemoveItem("mushroom_heal", 1);
        }
        
        // Act 5: Сохранение
        saveManager.SaveGame(gameState);
        
        // Act 6: Загрузка
        var loadedState = saveManager.LoadGame();
        
        // Assert
        loadedState.Should().NotBeNull();
        loadedState!.Player.Level.Should().BeGreaterThan(1);
        loadedState.Player.IsAlive.Should().BeTrue();
        loadedState.Inventory.GetItemCount("mushroom_heal").Should().Be(2); // Использовали 1 из 3
        
        // Cleanup
        Directory.Delete(tempDir, true);
    }
    
    [Fact]
    public void Collect_Items_Use_All_Should_Empty_Inventory()
    {
        // Arrange
        var gameState = new GameState();
        var itemRepository = new ItemRepository();
        var item = itemRepository.GetItem("mushroom_heal")!;
        
        // Act - собираем 3 зелья
        gameState.Inventory.AddItem(item, 3);
        
        // Используем все
        while (gameState.Inventory.HasItem("mushroom_heal"))
        {
            gameState.Player.Heal(item.HealthRestore);
            gameState.Inventory.RemoveItem("mushroom_heal", 1);
        }
        
        // Assert
        gameState.Inventory.HasItem("mushroom_heal").Should().BeFalse();
        gameState.Player.CurrentHealth.Should().Be(gameState.Player.MaxHealth);
    }
    
    [Fact]
    public void Dialogue_Choice_Should_Set_Flag_And_Give_Reward()
    {
        // Arrange
        var gameState = new GameState();
        var dialogueSystem = new DialogueSystem();
        var itemRepository = new ItemRepository();
        
        var scenario = new DialogueScenario
        {
            Id = "test",
            StartNodeId = "start",
            Nodes = new Dictionary<string, DialogueNode>
            {
                ["start"] = new DialogueNode
                {
                    Id = "start",
                    Text = "Test",
                    Options = new List<DialogueOption>
                    {
                        new()
                        {
                            Text = "Peaceful option",
                            NextNodeId = "",
                            Reward = new DialogueReward
                            {
                                Experience = 50,
                                ItemId = "mushroom_heal",
                                ItemQuantity = 2,
                                Flag = "peaceful_path"
                            },
                            Action = "end"
                        }
                    }
                }
            }
        };
        
        dialogueSystem.LoadScenario(scenario);
        var node = dialogueSystem.GetStartNode("test");
        var option = node!.Options[0];
        
        // Act
        dialogueSystem.ApplyReward(option.Reward, gameState.Player, gameState.Inventory, gameState, 
            itemRepository.GetAllItems().ToDictionary(i => i.Id));
        
        // Assert
        gameState.HasFlag("peaceful_path").Should().BeTrue();
        gameState.Inventory.HasItem("mushroom_heal", 2).Should().BeTrue();
        gameState.Player.Experience.Should().Be(50);
    }
    
    [Fact]
    public void Save_After_Battle_Should_Preserve_State()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var saveManager = new SaveGameManager(tempDir);
        var battleSystem = new BattleSystem();
        var gameState = new GameState();
        var enemy = new Enemy
        {
            MaxHealth = 30,
            CurrentHealth = 30,
            Defense = 0,
            ExperienceReward = 100
        };
        
        // Act - бой и повышение уровня
        while (enemy.IsAlive && gameState.Player.IsAlive)
        {
            battleSystem.PlayerAttack(gameState.Player, enemy);
            if (enemy.IsAlive)
            {
                battleSystem.EnemyTurn(gameState.Player, enemy);
            }
        }
        
        var reward = battleSystem.CalculateReward(enemy);
        gameState.Player.AddExperience(reward.Experience);
        
        saveManager.SaveGame(gameState);
        var loadedState = saveManager.LoadGame();
        
        // Assert
        loadedState.Should().NotBeNull();
        loadedState!.Player.Level.Should().Be(gameState.Player.Level);
        loadedState.Player.Experience.Should().Be(gameState.Player.Experience);
        loadedState.Player.CurrentHealth.Should().Be(gameState.Player.CurrentHealth);
        
        // Cleanup
        Directory.Delete(tempDir, true);
    }
}

