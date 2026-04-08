using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace StudentGradesDashboard.ViewModels
{
    /// <summary>
    /// Base class for all ViewModels in the application.
    /// Provides common functionality for property notification and error handling.
    /// </summary>
    public partial class BaseViewModel : ObservableObject
    {
        /// <summary>
        /// Gets or sets the error message to display to the user.
        /// </summary>
        [ObservableProperty]
        private string? errorMessage;

        /// <summary>
        /// Gets or sets a value indicating whether the view model is currently loading data.
        /// </summary>
        [ObservableProperty]
        private bool isLoading;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseViewModel"/> class.
        /// </summary>
        public BaseViewModel()
        {
            ErrorMessage = null;
            IsLoading = false;
        }

        /// <summary>
        /// Clears the current error message.
        /// </summary>
        public void ClearError()
        {
            ErrorMessage = null;
        }

        /// <summary>
        /// Sets an error message to display to the user.
        /// </summary>
        /// <param name="message">The error message to display.</param>
        public void SetError(string message)
        {
            ErrorMessage = message;
        }
    }
}
