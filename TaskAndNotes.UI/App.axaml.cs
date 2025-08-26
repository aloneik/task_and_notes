using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using System;
using Microsoft.Extensions.DependencyInjection;
using TaskAndNotes.Application.Services;
using TaskAndNotes.UI.ViewModels;

namespace TaskAndNotes.UI;

public partial class App : Avalonia.Application
{
    private readonly IServiceProvider _serviceProvider;

    public App(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var noteService = _serviceProvider.GetRequiredService<NoteService>();
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(noteService)
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}