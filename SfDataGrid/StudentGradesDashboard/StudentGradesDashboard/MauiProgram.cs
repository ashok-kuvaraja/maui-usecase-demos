using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Core.Hosting;
using StudentGradesDashboard.Services;
using StudentGradesDashboard.ViewModels;
using StudentGradesDashboard.Views;

namespace StudentGradesDashboard
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureSyncfusionCore()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .ConfigureServices();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

        /// <summary>
        /// Configures dependency injection services for the application.
        /// </summary>
        private static MauiAppBuilder ConfigureServices(this MauiAppBuilder builder)
        {
            // Register services
            builder.Services
                .AddSingleton<IDataService, DataService>()
                .AddSingleton<IValidationService, ValidationService>();

            // Register ViewModels
            builder.Services
                .AddTransient<DashboardViewModel>()
                .AddTransient<StudentDetailViewModel>();

            // Register Views
            builder.Services
                .AddTransient<DashboardPage>()
                .AddTransient<StudentDetailPage>();

            return builder;
        }
    }
}
