using TurtleHero.Core.Models;

namespace TurtleHero.Core.Game.Dialogue;

/// <summary>
/// Система управления диалогами
/// </summary>
public class DialogueSystem
{
    private readonly Dictionary<string, DialogueScenario> _scenarios = new();
    
    /// <summary>
    /// Загружает сценарий диалога
    /// </summary>
    public void LoadScenario(DialogueScenario scenario)
    {
        _scenarios[scenario.Id] = scenario;
    }
    
    /// <summary>
    /// Получает начальный узел диалога
    /// </summary>
    public DialogueNode? GetStartNode(string scenarioId)
    {
        if (!_scenarios.TryGetValue(scenarioId, out var scenario))
            return null;
        
        return GetNode(scenarioId, scenario.StartNodeId);
    }
    
    /// <summary>
    /// Получает узел диалога по ID
    /// </summary>
    public DialogueNode? GetNode(string scenarioId, string nodeId)
    {
        if (!_scenarios.TryGetValue(scenarioId, out var scenario))
            return null;
        
        return scenario.Nodes.TryGetValue(nodeId, out var node) ? node : null;
    }
    
    /// <summary>
    /// Проверяет доступность опции для выбора
    /// </summary>
    public bool IsOptionAvailable(DialogueOption option, Character player, Inventory inventory, GameState gameState)
    {
        if (option.Condition == null)
            return true;
        
        var condition = option.Condition;
        
        switch (condition.Type.ToLower())
        {
            case "strength":
                return CheckNumericCondition(player.Strength, condition);
            case "agility":
                return CheckNumericCondition(player.Agility, condition);
            case "defense":
                return CheckNumericCondition(player.Defense, condition);
            case "level":
                return CheckNumericCondition(player.Level, condition);
            case "has_item":
                if (condition.Value is string itemId)
                {
                    var hasItem = inventory.HasItem(itemId);
                    return condition.Operator == "==" ? hasItem : !hasItem;
                }
                return false;
            case "flag":
                if (condition.Value is string flag)
                {
                    var hasFlag = gameState.HasFlag(flag);
                    return condition.Operator == "==" ? hasFlag : !hasFlag;
                }
                return false;
            default:
                return true; // Неизвестное условие - показываем опцию
        }
    }
    
    private bool CheckNumericCondition(int playerValue, DialogueCondition condition)
    {
        if (condition.Value == null) return true;
        
        if (!int.TryParse(condition.Value.ToString(), out var requiredValue))
            return true;
        
        return condition.Operator switch
        {
            ">=" => playerValue >= requiredValue,
            "<=" => playerValue <= requiredValue,
            ">" => playerValue > requiredValue,
            "<" => playerValue < requiredValue,
            "==" => playerValue == requiredValue,
            "!=" => playerValue != requiredValue,
            _ => true
        };
    }
    
    /// <summary>
    /// Применяет награду за выбор опции
    /// </summary>
    public void ApplyReward(DialogueReward? reward, Character player, Inventory inventory, GameState gameState, Dictionary<string, Item> itemRepository)
    {
        if (reward == null) return;
        
        if (reward.Experience.HasValue)
        {
            player.AddExperience(reward.Experience.Value);
        }
        
        if (!string.IsNullOrEmpty(reward.ItemId) && reward.ItemQuantity.HasValue)
        {
            if (itemRepository.TryGetValue(reward.ItemId, out var item))
            {
                inventory.AddItem(item, reward.ItemQuantity.Value);
            }
        }
        
        if (!string.IsNullOrEmpty(reward.Flag))
        {
            gameState.SetFlag(reward.Flag);
        }
    }
}

