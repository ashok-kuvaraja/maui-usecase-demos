using Microsoft.Extensions.Logging;
using ProductCatalogViewerApp.Services;
using ProductCatalogViewerApp.ViewModels;
using ProductCatalogViewerApp.Views;
using Syncfusion.Maui.Core.Hosting;

namespace ProductCatalogViewerApp
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
                });

            // Register services
            builder.Services.AddSingleton<IProductService, ProductService>();

            // Register ViewModels
            builder.Services.AddTransient<ProductCatalogViewModel>();
            builder.Services.AddTransient<ProductDetailViewModel>();

            // Register Pages
            builder.Services.AddTransient<ProductCatalogPage>();
            builder.Services.AddTransient<ProductDetailPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
