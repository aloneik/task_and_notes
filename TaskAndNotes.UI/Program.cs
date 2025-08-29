using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
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

        var dbPath = GetDatabasePath();
        services.AddSingleton<INoteRepository>(_ => new LiteDbNoteRepository(dbPath));
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

    private static string GetDatabasePath()
    {
        var appDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "TaskAndNotes");
        Directory.CreateDirectory(appDir);
        return Path.Combine(appDir, "notes.db");
    }
}
