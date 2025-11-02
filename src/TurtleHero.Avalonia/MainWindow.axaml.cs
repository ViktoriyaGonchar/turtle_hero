using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using TurtleHero.Avalonia.Views;

namespace TurtleHero.Avalonia;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        this.KeyDown += MainWindow_KeyDown;
        
        // Показываем экран истории при запуске
        ShowStoryView();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void MainWindow_KeyDown(object? sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.H:
                ShowMainMenu();
                break;
            case Key.Z when e.KeyModifiers == KeyModifiers.Control:
                LoadGame();
                break;
            case Key.S when e.KeyModifiers == KeyModifiers.Control:
                SaveGame();
                break;
        }
    }

    private void ShowStoryView()
    {
        var storyView = new StoryView();
        storyView.OnStoryComplete += ShowMainMenu;
        ContentArea.Content = storyView;
        UpdateHint("Нажмите [Пробел] для продолжения...");
    }

    private void ShowMainMenu()
    {
        var menuView = new MainMenuView();
        menuView.OnNewGame += StartNewGame;
        menuView.OnLoadGame += LoadGame;
        menuView.OnExit += Close;
        ContentArea.Content = menuView;
        UpdateHint("Нажмите [Н] для нового приключения | [З] для загрузки | [В] для выхода");
    }

    private void StartNewGame()
    {
        var gameView = new GameView();
        ContentArea.Content = gameView;
        UpdateHint("Нажмите [I] для инвентаря | [С] для сохранения | [ESC] для меню");
    }

    private void LoadGame()
    {
        // TODO: Реализовать загрузку
        UpdateHint("Загрузка игры...");
    }

    private void SaveGame()
    {
        // TODO: Реализовать сохранение
        UpdateHint("Игра сохранена!");
    }

    private void UpdateHint(string hint)
    {
        if (HintText != null)
        {
            HintText.Text = hint;
        }
    }
}

