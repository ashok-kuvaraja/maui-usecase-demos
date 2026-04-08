using Microsoft.Extensions.DependencyInjection;

namespace SalesOrderTracker
{
    public partial class App : Application
    {
        public static IServiceProvider? Services { get; set; }

        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            // If the user hasn't logged in (prototype flag), show WelcomePage first
            var isLogged = Microsoft.Maui.Storage.Preferences.Default.Get("IsLoggedIn", false);
            if (!isLogged)
            {
                var page = App.Services?.GetService(typeof(Views.WelcomePage)) as Views.WelcomePage;
                if (page is null)
                {
                    page = new Views.WelcomePage();
                }

                return new Window(page);
            }

            return new Window(new AppShell());
        }
    }
}