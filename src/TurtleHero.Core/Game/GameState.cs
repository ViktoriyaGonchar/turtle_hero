using TurtleHero.Core.Models;

namespace TurtleHero.Core.Game;

/// <summary>
/// Основное состояние игры
/// </summary>
public class GameState
{
    public Character Player { get; set; } = new();
    public Inventory Inventory { get; set; } = new();
    
    // Прогресс игры
    public string CurrentLocation { get; set; } = "forest";
    public Dictionary<string, bool> GameFlags { get; set; } = new(); // Флаги для диалогов и событий
    
    // Метаданные сохранения
    public DateTime SaveTime { get; set; } = DateTime.Now;
    public string Version { get; set; } = "1.0.0";
    
    /// <summary>
    /// Проверяет флаг игры
    /// </summary>
    public bool HasFlag(string flag) => GameFlags.TryGetValue(flag, out var value) && value;
    
    /// <summary>
    /// Устанавливает флаг игры
    /// </summary>
    public void SetFlag(string flag, bool value = true)
    {
        GameFlags[flag] = value;
    }
    
    /// <summary>
    /// Сбрасывает состояние игры к начальному
    /// </summary>
    public void Reset()
    {
        Player = new Character();
        Inventory = new Inventory();
        CurrentLocation = "forest";
        GameFlags.Clear();
        SaveTime = DateTime.Now;
    }
}

