using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Avalonia;

namespace TurtleHero.Avalonia;

internal class Program
{
    // Для отображения консоли в WinExe приложении на Windows
    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool AllocConsole();
    
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        // Создаём консоль для отладки (WinExe не показывает консоль по умолчанию)
        if (OperatingSystem.IsWindows())
        {
            try
            {
                AllocConsole();
            }
            catch
            {
                // Игнорируем, если консоль уже есть
            }
        }
        
        // ОБЯЗАТЕЛЬНЫЙ вывод в самом начале - проверяем, что Main вызывается
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║  🐢 Черепашка-герой - Запуск...                           ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
        Console.WriteLine($"[DEBUG] Main вызван. Args: {string.Join(" ", args)}");
        Console.WriteLine($"[DEBUG] OS: {Environment.OSVersion}");
        Console.WriteLine($"[DEBUG] IsWindows: {OperatingSystem.IsWindows()}");
        Console.WriteLine($"[DEBUG] DOTNET_RUNNING_IN_CONTAINER: {Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER")}");
        Console.WriteLine($"[DEBUG] WSL_DISTRO_NAME: {Environment.GetEnvironmentVariable("WSL_DISTRO_NAME")}");
        Console.WriteLine($"[DEBUG] DISPLAY: {Environment.GetEnvironmentVariable("DISPLAY")}");
        Console.WriteLine();
        
        // Проверка окружения перед запуском GUI
        bool guiAvailable = IsGuiEnvironmentAvailable();
        Console.WriteLine($"[DEBUG] IsGuiEnvironmentAvailable: {guiAvailable}");
        Console.WriteLine();
        
        if (!guiAvailable)
        {
            Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║  ⚠️  ОШИБКА: Графическое окружение недоступно!            ║");
            Console.WriteLine("╠════════════════════════════════════════════════════════════╣");
            Console.WriteLine("║  Avalonia UI требует графическую среду для работы.        ║");
            Console.WriteLine("║                                                            ║");
            Console.WriteLine("║  Возможные причины:                                        ║");
            Console.WriteLine("║  • Запуск в Docker контейнере без X11/Wayland             ║");
            Console.WriteLine("║  • Отсутствует дисплей на сервере                         ║");
            Console.WriteLine("║  • Windows: нет доступа к Desktop API                     ║");
            Console.WriteLine("║                                                            ║");
            Console.WriteLine("║  Решения:                                                  ║");
            Console.WriteLine("║  • Запустите на Windows напрямую (не в Docker):           ║");
            Console.WriteLine("║    dotnet run --project src/TurtleHero.Avalonia/...       ║");
            Console.WriteLine("║  • Для Docker: используйте только Core и тесты            ║");
            Console.WriteLine("║  • Для Linux: настройте X11 forwarding                    ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
            Console.WriteLine();
            Console.WriteLine("Нажмите любую клавишу для выхода...");
            try { Console.ReadKey(); } catch { }
            Environment.Exit(1);
            return;
        }

        try
        {
            Console.WriteLine("[DEBUG] Начинаем инициализацию Avalonia...");
            var app = BuildAvaloniaApp();
            Console.WriteLine("[DEBUG] Avalonia сконфигурирован, запускаем GUI...");
            Console.WriteLine("[DEBUG] Примечание: После этого сообщения GUI должен открыться.");
            Console.WriteLine("[DEBUG] Если ничего не происходит, возможно проблема с Avalonia.");
            Console.WriteLine();
            
            // Отключаем вывод в консоль после запуска GUI (чтобы не мешать)
            app.StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║  ❌ ОШИБКА ПРИ ЗАПУСКЕ AVALONIA                            ║");
            Console.WriteLine("╠════════════════════════════════════════════════════════════╣");
            var message = ex.Message.Length > 56 ? ex.Message.Substring(0, 53) + "..." : ex.Message;
            Console.WriteLine($"║  {message,-56} ║");
            Console.WriteLine("╠════════════════════════════════════════════════════════════╣");
            Console.WriteLine("║  Детали:                                                    ║");
            var typeName = ex.GetType().Name.Length > 56 ? ex.GetType().Name.Substring(0, 53) + "..." : ex.GetType().Name;
            Console.WriteLine($"║  {typeName,-56} ║");
            if (!string.IsNullOrEmpty(ex.StackTrace))
            {
                var stackLines = ex.StackTrace.Split('\n').Take(5);
                foreach (var line in stackLines)
                {
                    var trimmed = line.Trim();
                    if (trimmed.Length > 56) trimmed = trimmed.Substring(0, 53) + "...";
                    Console.WriteLine($"║    {trimmed,-54} ║");
                }
            }
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
            
            // Дополнительная информация для отладки
            Console.WriteLine("\n[DEBUG] Дополнительная информация:");
            Console.WriteLine($"  OS: {Environment.OSVersion}");
            Console.WriteLine($"  Platform: {Environment.OSVersion.Platform}");
            Console.WriteLine($"  IsWindows: {OperatingSystem.IsWindows()}");
            Console.WriteLine($"  IsLinux: {OperatingSystem.IsLinux()}");
            Console.WriteLine($"  DOTNET_RUNNING_IN_CONTAINER: {Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER")}");
            
            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
            Environment.Exit(1);
        }
    }

    /// <summary>
    /// Проверяет, доступна ли графическая среда для запуска GUI
    /// </summary>
    private static bool IsGuiEnvironmentAvailable()
    {
        // На Windows проверяем наличие консоли и доступность GUI API
        if (OperatingSystem.IsWindows())
        {
            // В Docker на Windows обычно нет доступа к GUI
            // Проверяем наличие переменной окружения, указывающей на Docker
            if (Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true")
            {
                return false;
            }
            
            // Проверяем наличие DISPLAY (для WSL2 с X11)
            var display = Environment.GetEnvironmentVariable("DISPLAY");
            if (string.IsNullOrEmpty(display))
            {
                // На обычном Windows это нормально, но в Docker/WSL может быть проблемой
                // Попробуем определить, находимся ли мы в контейнере
                try
                {
                    // Если есть /.dockerenv или /proc/self/cgroup - мы в Docker
                    if (File.Exists("/.dockerenv") || 
                        (Directory.Exists("/proc") && File.Exists("/proc/1/cgroup") && 
                         File.ReadAllText("/proc/1/cgroup").Contains("docker")))
                    {
                        return false;
                    }
                }
                catch
                {
                    // Игнорируем ошибки доступа к файлам
                }
            }
            
            return true; // На обычном Windows предполагаем, что GUI доступен
        }
        
        // На Linux/Unix проверяем DISPLAY
        if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
        {
            var display = Environment.GetEnvironmentVariable("DISPLAY");
            if (string.IsNullOrEmpty(display))
            {
                // Проверяем Wayland
                var waylandDisplay = Environment.GetEnvironmentVariable("WAYLAND_DISPLAY");
                if (string.IsNullOrEmpty(waylandDisplay))
                {
                    return false;
                }
            }
            return true;
        }
        
        // Для других платформ предполагаем недоступность
        return false;
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}

