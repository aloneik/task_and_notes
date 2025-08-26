using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using System;
using TaskAndNotes.Application.Services;
using TaskAndNotes.Domain.Abstractions;
using TaskAndNotes.Infrastructure.Persistence;
using TaskAndNotes.Infrastructure.Sync;

namespace TaskAndNotes.UI;

class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        var services = new ServiceCollection();

        services.AddSingleton<INoteRepository>(_ => new LiteDbNoteRepository("notes.db"));
        services.AddSingleton<ISyncProvider, NoOpSyncProvider>();
        services.AddSingleton<NoteService>();

        var provider = services.BuildServiceProvider();

        BuildAvaloniaApp(provider)
            .StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp(IServiceProvider serviceProvider)
        => AppBuilder.Configure(() => new App(serviceProvider))
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
