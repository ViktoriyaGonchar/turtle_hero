using System.Text.Json;
using TurtleHero.Core.Game.Dialogue;

namespace TurtleHero.Core.Storage;

/// <summary>
/// Загрузчик сценариев диалогов из JSON
/// </summary>
public class ScenarioLoader
{
    /// <summary>
    /// Загружает сценарий из JSON файла
    /// </summary>
    public DialogueScenario? LoadFromFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Файл сценария не найден: {filePath}");
        }
        
        try
        {
            var json = File.ReadAllText(filePath);
            return LoadFromJson(json);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"Ошибка парсинга JSON сценария: {ex.Message}", ex);
        }
        catch (FileNotFoundException)
        {
            throw; // Пробрасываем FileNotFoundException наверх
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Ошибка загрузки сценария: {ex.Message}", ex);
        }
    }
    
    /// <summary>
    /// Загружает сценарий из JSON строки
    /// </summary>
    public DialogueScenario? LoadFromJson(string json)
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            
            var scenario = JsonSerializer.Deserialize<DialogueScenario>(json, options);
            
            if (scenario == null)
            {
                throw new InvalidOperationException("Не удалось десериализовать сценарий");
            }
            
            // Валидация сценария
            ValidateScenario(scenario);
            
            return scenario;
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"Ошибка парсинга JSON: {ex.Message}", ex);
        }
    }
    
    /// <summary>
    /// Валидирует загруженный сценарий
    /// </summary>
    private void ValidateScenario(DialogueScenario scenario)
    {
        if (string.IsNullOrEmpty(scenario.Id))
        {
            throw new InvalidOperationException("Сценарий должен иметь ID");
        }
        
        if (string.IsNullOrEmpty(scenario.StartNodeId))
        {
            throw new InvalidOperationException("Сценарий должен иметь StartNodeId");
        }
        
        if (!scenario.Nodes.ContainsKey(scenario.StartNodeId))
        {
            throw new InvalidOperationException($"Стартовый узел '{scenario.StartNodeId}' не найден в сценарии");
        }
        
        // Проверяем, что все ссылки на узлы существуют
        foreach (var node in scenario.Nodes.Values)
        {
            foreach (var option in node.Options)
            {
                if (!string.IsNullOrEmpty(option.NextNodeId) && !scenario.Nodes.ContainsKey(option.NextNodeId))
                {
                    throw new InvalidOperationException($"Узел '{option.NextNodeId}' не найден в сценарии (ссылка из узла '{node.Id}')");
                }
            }
        }
    }
}

