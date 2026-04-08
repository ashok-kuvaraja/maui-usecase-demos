using ProductCatalogViewerApp.Views;

namespace ProductCatalogViewerApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Register routes for navigation
            Routing.RegisterRoute("details", typeof(ProductDetailPage));
        }
    }
}
