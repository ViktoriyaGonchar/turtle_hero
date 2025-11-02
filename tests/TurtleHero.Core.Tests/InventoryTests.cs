using FluentAssertions;
using TurtleHero.Core.Models;
using Xunit;

namespace TurtleHero.Core.Tests;

public class InventoryTests
{
    [Fact]
    public void Inventory_AddItem_Should_Increase_Quantity()
    {
        // Arrange
        var inventory = new Inventory();
        var item = new Item { Id = "test_item", MaxStack = 99 };
        
        // Act
        inventory.AddItem(item, 5);
        
        // Assert
        inventory.GetItemCount("test_item").Should().Be(5);
    }
    
    [Fact]
    public void Inventory_AddItem_Should_Create_Stack_For_New_Item()
    {
        // Arrange
        var inventory = new Inventory();
        var item = new Item { Id = "new_item", MaxStack = 99 };
        
        // Act
        var result = inventory.AddItem(item, 3);
        
        // Assert
        result.Should().BeTrue();
        inventory.HasItem("new_item", 3).Should().BeTrue();
    }
    
    [Fact]
    public void Inventory_RemoveItem_Should_Decrease_Quantity()
    {
        // Arrange
        var inventory = new Inventory();
        var item = new Item { Id = "test_item", MaxStack = 99 };
        inventory.AddItem(item, 10);
        
        // Act
        inventory.RemoveItem("test_item", 3);
        
        // Assert
        inventory.GetItemCount("test_item").Should().Be(7);
    }
    
    [Fact]
    public void Inventory_RemoveItem_Should_Remove_Stack_When_Quantity_Zero()
    {
        // Arrange
        var inventory = new Inventory();
        var item = new Item { Id = "test_item", MaxStack = 99 };
        inventory.AddItem(item, 5);
        
        // Act
        inventory.RemoveItem("test_item", 5);
        
        // Assert
        inventory.HasItem("test_item").Should().BeFalse();
        inventory.GetItemStack("test_item").Should().BeNull();
    }
    
    [Fact]
    public void Inventory_Should_Not_Exceed_MaxSlots()
    {
        // Arrange
        var inventory = new Inventory();
        
        // Act - добавляем предметы до максимума
        for (int i = 0; i < Inventory.MaxSlots; i++)
        {
            var item = new Item { Id = $"item_{i}", MaxStack = 1 };
            inventory.AddItem(item);
        }
        
        // Попытка добавить ещё один
        var extraItem = new Item { Id = "extra_item", MaxStack = 1 };
        var result = inventory.AddItem(extraItem);
        
        // Assert
        result.Should().BeFalse();
        inventory.HasItem("extra_item").Should().BeFalse();
    }
    
    [Fact]
    public void Inventory_AddItem_Should_Stack_Existing_Items()
    {
        // Arrange
        var inventory = new Inventory();
        var item = new Item { Id = "stackable_item", MaxStack = 99 };
        inventory.AddItem(item, 5);
        
        // Act
        inventory.AddItem(item, 3);
        
        // Assert
        inventory.GetItemCount("stackable_item").Should().Be(8);
        inventory.UsedSlots.Should().Be(1); // Всё ещё один слот
    }
    
    [Fact]
    public void Inventory_HasItem_Should_Return_True_When_Enough_Quantity()
    {
        // Arrange
        var inventory = new Inventory();
        var item = new Item { Id = "test_item", MaxStack = 99 };
        inventory.AddItem(item, 10);
        
        // Act & Assert
        inventory.HasItem("test_item", 5).Should().BeTrue();
        inventory.HasItem("test_item", 10).Should().BeTrue();
        inventory.HasItem("test_item", 11).Should().BeFalse();
    }
    
    [Fact]
    public void Inventory_RemoveItem_Should_Return_False_When_Not_Enough()
    {
        // Arrange
        var inventory = new Inventory();
        var item = new Item { Id = "test_item", MaxStack = 99 };
        inventory.AddItem(item, 5);
        
        // Act
        var result = inventory.RemoveItem("test_item", 10);
        
        // Assert
        result.Should().BeFalse();
        inventory.GetItemCount("test_item").Should().Be(5); // Количество не изменилось
    }
    
    [Fact]
    public void Inventory_Consumable_Item_Should_Restore_Health()
    {
        // Arrange
        var character = new Character { CurrentHealth = 30, MaxHealth = 50 };
        var item = new Item { Id = "heal_potion", HealthRestore = 20 };
        
        // Act
        character.Heal(item.HealthRestore);
        
        // Assert
        character.CurrentHealth.Should().Be(50); // Не больше максимума
    }
}

