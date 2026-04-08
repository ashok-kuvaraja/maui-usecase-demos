using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Storage;

namespace SalesOrderTracker.ViewModels
{
    public partial class WelcomeViewModel : ObservableObject
    {
        public WelcomeViewModel()
        {
            Email = string.Empty;
            Password = string.Empty;
            RememberMe = false;
        }

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private bool rememberMe;

        public ICommand LoginCommand => new AsyncRelayCommand(LoginAsync);

        private async System.Threading.Tasks.Task LoginAsync()
        {
            // For prototype any credentials are accepted.
            if (RememberMe)
            {
                Preferences.Default.Set("IsLoggedIn", true);
                Preferences.Default.Set("LastLoginEmail", Email ?? string.Empty);
            }
            else
            {
                // ensure no persistent login
                Preferences.Default.Set("IsLoggedIn", false);
            }

            // small delay to allow UI feedback
            await System.Threading.Tasks.Task.Delay(150);
        }
    }
}
