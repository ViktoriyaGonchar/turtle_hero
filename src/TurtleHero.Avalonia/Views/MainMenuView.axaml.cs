using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace TurtleHero.Avalonia.Views;

public partial class MainMenuView : UserControl
{
    public event Action? OnNewGame;
    public event Action? OnLoadGame;
    public event Action? OnExit;

    public MainMenuView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void NewGameButton_Click(object? sender, RoutedEventArgs e)
    {
        OnNewGame?.Invoke();
    }

    private void LoadGameButton_Click(object? sender, RoutedEventArgs e)
    {
        OnLoadGame?.Invoke();
    }

    private void ExitButton_Click(object? sender, RoutedEventArgs e)
    {
        OnExit?.Invoke();
    }
}

