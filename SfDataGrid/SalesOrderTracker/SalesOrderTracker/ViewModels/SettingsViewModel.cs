using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Syncfusion.Maui.Themes;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace SalesOrderTracker.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        public SettingsViewModel()
        {
            // initialize values
            AppVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0";
            _enableSampleData = Preferences.Default.Get("EnableSampleData", true);
            _useDummyJson = Preferences.Default.Get("UseDummyJson", true);

            // Theme
            SelectedTheme = Preferences.Default.Get("AppTheme", "System");
            // Apply persisted values on startup
            ApplyTheme(SelectedTheme);
        }

        [ObservableProperty]
        private string appVersion = string.Empty;

        // Backing field used to avoid invoking Preferences every get
        private bool _enableSampleData;
        private bool _useDummyJson;

        public bool EnableSampleData
        {
            get => _enableSampleData;
            set
            {
                if (SetProperty(ref _enableSampleData, value))
                {
                    Preferences.Default.Set("EnableSampleData", value);
                }
            }
        }

        public bool UseDummyJson
        {
            get => _useDummyJson;
            set
            {
                if (SetProperty(ref _useDummyJson, value))
                {
                    Preferences.Default.Set("UseDummyJson", value);
                }
            }
        }

        [ObservableProperty]
        private string selectedTheme = "System";

        partial void OnSelectedThemeChanged(string value)
        {
            try
            {
                Preferences.Default.Set("AppTheme", value ?? "System");
                ApplyTheme(value);
            }
            catch { }
        }

        private void ApplyTheme(string? value)
        {
            try
            {
                var app = Application.Current;
                if (app == null)
                    return;

                var mode = (value ?? "System").ToLowerInvariant();
                switch (mode)
                {
                    case "light":
                        app.UserAppTheme = AppTheme.Light;
                        break;
                    case "dark":
                        app.UserAppTheme = AppTheme.Dark;
                        break;
                    default:
                        app.UserAppTheme = AppTheme.Unspecified;
                        break;
                }

                // Try switching Syncfusion theme resource dictionary via reflection (no compile-time dependency)
                try
                {
                    ICollection<ResourceDictionary> mergedDictionaries = Application.Current.Resources.MergedDictionaries;
                    if (mode == "dark") 
                    {                        
                        if (mergedDictionaries != null)
                        {
                            var theme = mergedDictionaries.OfType<SyncfusionThemeResourceDictionary>().FirstOrDefault();
                            if (theme != null)
                            {
                                theme.VisualTheme = SfVisuals.MaterialDark;
                            }
                        }
                    }
                    else 
                    {
                        if (mergedDictionaries != null)
                        {
                            var theme = mergedDictionaries.OfType<SyncfusionThemeResourceDictionary>().FirstOrDefault();
                            if (theme != null)
                            {
                                theme.VisualTheme = SfVisuals.MaterialLight;
                            }
                        }
                    }
                    
                }
                catch { }

                    // Update app color resources (Colors.xaml) so styles update immediately
                    try
                    {
                        var merged = Application.Current?.Resources?.MergedDictionaries;
                        if (merged != null)
                        {
                            // Find the colors resource dictionary (by Source or by key presence)
                            ResourceDictionary colorsDict = null;
                            foreach (var d in merged)
                            {
                                // resource dictionaries loaded from XAML have a Source property
                                var rd = d as ResourceDictionary;
                                var src = rd?.Source?.OriginalString;
                                if (!string.IsNullOrEmpty(src) && src.IndexOf("Colors.xaml", StringComparison.OrdinalIgnoreCase) >= 0)
                                {
                                    colorsDict = rd;
                                    break;
                                }
                                // fallback: detect by known keys
                                if (rd != null && rd.ContainsKey("Primary") && rd.ContainsKey("AppBackgroundBrush"))
                                {
                                    colorsDict = rd;
                                    break;
                                }
                            }

                            if (colorsDict != null)
                            {
                                if (mode == "dark")
                                {
                                    // Dark palette adjustments
                                    colorsDict["BackgroundColor"] = Color.FromArgb("#0F0F0F");
                                    colorsDict["AppBackgroundBrush"] = new LinearGradientBrush(
                                        new GradientStopCollection {
                                            new GradientStop { Color = Color.FromArgb("#0B0B0B"), Offset = 0.0f },
                                            new GradientStop { Color = Color.FromArgb("#111111"), Offset = 1.0f }
                                        },
                                        new Point(0,0), new Point(1,1));
                                    colorsDict["CardBackgroundBrush"] = new SolidColorBrush(Color.FromArgb("#1E1E1E"));
                                    colorsDict["TextPrimary"] = Color.FromArgb("#FFFFFF");
                                    colorsDict["TextSecondary"] = Color.FromArgb("#CCCCCC");
                                    colorsDict["PrimaryColor"] = Color.FromArgb("#8F7AE6");
                                }
                                else if (mode == "light")
                                {
                                    // Restore light values (simple defaults)
                                    colorsDict["BackgroundColor"] = Color.FromArgb("#FFFFFF");
                                    colorsDict["AppBackgroundBrush"] = new LinearGradientBrush(
                                        new GradientStopCollection {
                                            new GradientStop { Color = Color.FromArgb("#FFF8FF"), Offset = 0.0f },
                                            new GradientStop { Color = Color.FromArgb("#F6F5FF"), Offset = 0.5f },
                                            new GradientStop { Color = Color.FromArgb("#FFF8FB"), Offset = 1.0f }
                                        },
                                        new Point(0,0), new Point(1,1));
                                    colorsDict["CardBackgroundBrush"] = new SolidColorBrush(Color.FromArgb("#FFFFFFFF"));
                                    colorsDict["TextPrimary"] = Color.FromArgb("#111827");
                                    colorsDict["TextSecondary"] = Color.FromArgb("#6B7280");
                                    colorsDict["PrimaryColor"] = Color.FromArgb("#512BD4");
                                }
                            }
                        }
                    }
                    catch { }
            }
            catch { }
        }
    }
}
