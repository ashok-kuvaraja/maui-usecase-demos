using System.Collections.ObjectModel;

namespace SalesOrderTracker.Views
{
    public partial class SettingsPage : ContentPage
    {
        private readonly ViewModels.SettingsViewModel _vm;

        public SettingsPage()
        {
            InitializeComponent();

            // Resolve ViewModel from DI
            _vm = App.Services?.GetService(typeof(ViewModels.SettingsViewModel)) as ViewModels.SettingsViewModel
                             ?? new ViewModels.SettingsViewModel();
            BindingContext = _vm;

            // Theme selection is bound to ViewModel via Picker in XAML
        }

        private void OnLogoutClicked(object sender, EventArgs e)
        {
            try
            {
                Microsoft.Maui.Storage.Preferences.Default.Set("IsLoggedIn", false);
            }
            catch { }

            // Navigate back to welcome page
            var window = Application.Current?.Windows?.Count > 0 ? Application.Current.Windows[0] : null;
            if (window != null)
            {
                window.Page = new Views.WelcomePage();
            }
            else
            {
                Application.Current.MainPage = new Views.WelcomePage();
            }
        }

        private void Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }
    }
}
