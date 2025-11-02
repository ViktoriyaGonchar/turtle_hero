using System.Text.Json;
using System.Text.Json.Serialization;
using TurtleHero.Core.Game;
using TurtleHero.Core.Models;

namespace TurtleHero.Core.Storage;

/// <summary>
/// Менеджер сохранения и загрузки игры
/// </summary>
public class SaveGameManager
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };
    
    private readonly string _saveDirectory;
    private const string SaveFileName = "savegame.turtle";
    
    public SaveGameManager(string? saveDirectory = null)
    {
        // По умолчанию используем AppData/Local/TurtleHero
        _saveDirectory = saveDirectory ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "TurtleHero",
            "Saves"
        );
        
        // Создаём директорию, если её нет
        Directory.CreateDirectory(_saveDirectory);
    }
    
    public string SaveFilePath => Path.Combine(_saveDirectory, SaveFileName);
    
    /// <summary>
    /// Сохраняет состояние игры
    /// </summary>
    public bool SaveGame(GameState gameState)
    {
        try
        {
            gameState.SaveTime = DateTime.Now;
            
            var json = JsonSerializer.Serialize(gameState, JsonOptions);
            File.WriteAllText(SaveFilePath, json);
            
            return true;
        }
        catch (Exception ex)
        {
            // Логирование ошибки (в реальном приложении использовать ILogger)
            Console.WriteLine($"Ошибка сохранения: {ex.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// Загружает состояние игры
    /// </summary>
    public GameState? LoadGame()
    {
        try
        {
            if (!File.Exists(SaveFilePath))
            {
                return null;
            }
            
            var json = File.ReadAllText(SaveFilePath);
            var gameState = JsonSerializer.Deserialize<GameState>(json, JsonOptions);
            
            if (gameState == null)
            {
                return null;
            }
            
            // Валидация загруженных данных
            ValidateGameState(gameState);
            
            return gameState;
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Ошибка парсинга JSON: {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка загрузки: {ex.Message}");
            return null;
        }
    }
    
    /// <summary>
    /// Проверяет наличие сохранения
    /// </summary>
    public bool SaveExists()
    {
        return File.Exists(SaveFilePath);
    }
    
    /// <summary>
    /// Удаляет сохранение
    /// </summary>
    public bool DeleteSave()
    {
        try
        {
            if (File.Exists(SaveFilePath))
            {
                File.Delete(SaveFilePath);
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка удаления сохранения: {ex.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// Валидирует загруженное состояние игры
    /// </summary>
    private void ValidateGameState(GameState gameState)
    {
        // Восстанавливаем минимальные значения, если что-то не так
        if (gameState.Player == null)
        {
            gameState.Player = new Character();
        }
        
        if (gameState.Player.Level < 1)
        {
            gameState.Player.Level = 1;
        }
        
        if (gameState.Player.MaxHealth <= 0)
        {
            gameState.Player.MaxHealth = 50;
        }
        
        if (gameState.Player.CurrentHealth < 0)
        {
            gameState.Player.CurrentHealth = 0;
        }
        
        if (gameState.Player.CurrentHealth > gameState.Player.MaxHealth)
        {
            gameState.Player.CurrentHealth = gameState.Player.MaxHealth;
        }
        
        if (gameState.Inventory == null)
        {
            gameState.Inventory = new Inventory();
        }
        
        if (gameState.GameFlags == null)
        {
            gameState.GameFlags = new Dictionary<string, bool>();
        }
        
        if (string.IsNullOrEmpty(gameState.CurrentLocation))
        {
            gameState.CurrentLocation = "forest";
        }
    }
}

