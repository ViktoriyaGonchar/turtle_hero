using FluentAssertions;
using TurtleHero.Core.Game;
using TurtleHero.Core.Game.Dialogue;
using TurtleHero.Core.Models;
using Xunit;

namespace TurtleHero.Core.Tests;

public class DialogueSystemTests
{
    [Fact]
    public void DialogueSystem_Option_With_Strength_Condition_Should_Check_Correctly()
    {
        // Arrange
        var dialogueSystem = new DialogueSystem();
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
                            Text = "Option 1",
                            Condition = new DialogueCondition { Type = "strength", Operator = ">=", Value = 5 }
                        }
                    }
                }
            }
        };
        
        dialogueSystem.LoadScenario(scenario);
        var player = new Character { Strength = 10 };
        var inventory = new Inventory();
        var gameState = new GameState { Player = player };
        
        var node = dialogueSystem.GetStartNode("test");
        
        // Act & Assert
        var option = node!.Options[0];
        dialogueSystem.IsOptionAvailable(option, player, inventory, gameState).Should().BeTrue();
        
        // С низкой силой опция недоступна
        player.Strength = 3;
        dialogueSystem.IsOptionAvailable(option, player, inventory, gameState).Should().BeFalse();
    }
    
    [Fact]
    public void DialogueSystem_Option_With_HasItem_Condition_Should_Check_Correctly()
    {
        // Arrange
        var dialogueSystem = new DialogueSystem();
        var player = new Character();
        var inventory = new Inventory();
        inventory.AddItem(new Item { Id = "test_item" }, 1);
        var gameState = new GameState();
        
        var option = new DialogueOption
        {
            Text = "Use item",
            Condition = new DialogueCondition { Type = "has_item", Operator = "==", Value = "test_item" }
        };
        
        // Act & Assert
        dialogueSystem.IsOptionAvailable(option, player, inventory, gameState).Should().BeTrue();
        
        inventory.RemoveItem("test_item");
        dialogueSystem.IsOptionAvailable(option, player, inventory, gameState).Should().BeFalse();
    }
    
    [Fact]
    public void DialogueSystem_LoadScenario_Should_Store_Scenario()
    {
        // Arrange
        var dialogueSystem = new DialogueSystem();
        var scenario = new DialogueScenario
        {
            Id = "test_scenario",
            StartNodeId = "start",
            Nodes = new Dictionary<string, DialogueNode>
            {
                ["start"] = new DialogueNode { Id = "start", Text = "Hello" }
            }
        };
        
        // Act
        dialogueSystem.LoadScenario(scenario);
        var node = dialogueSystem.GetStartNode("test_scenario");
        
        // Assert
        node.Should().NotBeNull();
        node!.Text.Should().Be("Hello");
    }
    
    [Fact]
    public void DialogueSystem_GetNode_Should_Return_Correct_Node()
    {
        // Arrange
        var dialogueSystem = new DialogueSystem();
        var scenario = new DialogueScenario
        {
            Id = "test",
            StartNodeId = "start",
            Nodes = new Dictionary<string, DialogueNode>
            {
                ["start"] = new DialogueNode { Id = "start", Text = "Start" },
                ["next"] = new DialogueNode { Id = "next", Text = "Next" }
            }
        };
        
        dialogueSystem.LoadScenario(scenario);
        
        // Act
        var node = dialogueSystem.GetNode("test", "next");
        
        // Assert
        node.Should().NotBeNull();
        node!.Text.Should().Be("Next");
    }
}

