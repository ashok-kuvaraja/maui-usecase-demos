using Microsoft.Maui.Controls;
using SalesOrderTracker.ViewModels;
using Microsoft.Maui.Storage;

namespace SalesOrderTracker.Views
{
    public partial class WelcomePage : ContentPage
    {
        private readonly WelcomeViewModel _vm;

        public WelcomePage()
        {
            InitializeComponent();
            _vm = App.Services?.GetService(typeof(WelcomeViewModel)) as WelcomeViewModel ?? new WelcomeViewModel();
            BindingContext = _vm;

            // Hook into login command completion to navigate to AppShell
            // Since LoginCommand is AsyncRelayCommand, we await the task when button pressed via event handler
            // but for simplicity observe Preferences change on appearing
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // If already logged in, go to AppShell
            var isLogged = Preferences.Default.Get("IsLoggedIn", false);
            if (isLogged)
            {
                Application.Current.MainPage = new AppShell();
                return;
            }

            // Attach handler to the Login button command via MessagingCenter pattern: observe Preferences after login
            // We'll attach to the Login button click to perform navigation after command completes
            var loginButton = this.FindByName<Button>("LoginButton");
            if (loginButton != null)
            {
                loginButton.Clicked += async (s, e) =>
                {
                    // Execute the command (it will set Preferences if RememberMe is true)
                    if (_vm.LoginCommand.CanExecute(null))
                    {
                        await (_vm.LoginCommand as CommunityToolkit.Mvvm.Input.AsyncRelayCommand)?.ExecuteAsync(null);

                        // Navigate to AppShell for the demo regardless of persistence
                        var window = Application.Current?.Windows?.Count > 0 ? Application.Current.Windows[0] : null;
                        if (window != null)
                        {
                            window.Page = new AppShell();
                        }
                        else
                        {
                            Application.Current.MainPage = new AppShell();
                        }
                    }
                };
            }
        }
    }
}
