using Microsoft.Extensions.Logging;
using PracticeTracker.Services;
using SQLite;

namespace PracticeTracker;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "practice_tracker.db3");

        builder.Services.AddSingleton(new SQLiteAsyncConnection(dbPath));
        builder.Services.AddSingleton<DatabaseService>();
        builder.Services.AddTransient<MainPage>();

        return builder.Build();
    }
}