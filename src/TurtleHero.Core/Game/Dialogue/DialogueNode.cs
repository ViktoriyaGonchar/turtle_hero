namespace TurtleHero.Core.Game.Dialogue;

/// <summary>
/// Узел диалога
/// </summary>
public class DialogueNode
{
    public string Id { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public string Speaker { get; set; } = string.Empty;
    public string Emoji { get; set; } = "💬";
    public List<DialogueOption> Options { get; set; } = new();
}

/// <summary>
/// Вариант ответа в диалоге
/// </summary>
public class DialogueOption
{
    public string Text { get; set; } = string.Empty;
    public string NextNodeId { get; set; } = string.Empty;
    public DialogueCondition? Condition { get; set; }
    public DialogueReward? Reward { get; set; }
    public string? Action { get; set; } // Например: "battle", "shop", "end"
    public string? ActionParameter { get; set; } // Параметр действия (ID врага, и т.д.)
}

/// <summary>
/// Условие для показа опции
/// </summary>
public class DialogueCondition
{
    public string Type { get; set; } = string.Empty; // "strength", "has_item", "flag"
    public string Operator { get; set; } = ">="; // ">=", "<=", "==", "!="
    public object? Value { get; set; }
}

/// <summary>
/// Награда за выбор опции
/// </summary>
public class DialogueReward
{
    public int? Experience { get; set; }
    public string? ItemId { get; set; }
    public int? ItemQuantity { get; set; }
    public string? Flag { get; set; } // Устанавливаемый флаг
}

/// <summary>
/// Сценарий диалога (вся ветка диалога)
/// </summary>
public class DialogueScenario
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string StartNodeId { get; set; } = string.Empty;
    public Dictionary<string, DialogueNode> Nodes { get; set; } = new();
}

