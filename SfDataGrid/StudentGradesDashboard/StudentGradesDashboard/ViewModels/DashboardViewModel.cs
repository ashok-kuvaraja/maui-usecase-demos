using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using StudentGradesDashboard.Models;
using StudentGradesDashboard.Services;

namespace StudentGradesDashboard.ViewModels
{
    /// <summary>
    /// ViewModel for the Dashboard page.
    /// Manages student data display, filtering, and interactions.
    /// </summary>
    public partial class DashboardViewModel : BaseViewModel
    {
        private readonly IDataService _dataService;
        private readonly IValidationService _validationService;

        /// <summary>
        /// Gets or sets the collection of all students.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<Student> students = new();

        /// <summary>
        /// Gets or sets the filtered/displayed collection of students.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<Student> filteredStudents = new();

        /// <summary>
        /// Gets or sets the current filter/search term.
        /// </summary>
        [ObservableProperty]
        private string? filterTerm;

        /// <summary>
        /// Gets or sets the minimum grade filter (0-100).
        /// </summary>
        [ObservableProperty]
        private decimal minGradeFilter = 0;

        /// <summary>
        /// Gets or sets the maximum grade filter (0-100).
        /// </summary>
        [ObservableProperty]
        private decimal maxGradeFilter = 100;

        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardViewModel"/> class.
        /// </summary>
        /// <param name="dataService">Service for data access.</param>
        /// <param name="validationService">Service for data validation.</param>
        /// <param name="exportService">Service for data export.</param>
        public DashboardViewModel(IDataService dataService, IValidationService validationService)
        {
            _dataService = dataService;
            _validationService = validationService;
        }

        /// <summary>
        /// Initializes the view model by loading student data.
        /// Should be called from OnAppearing in the view.
        /// </summary>
        [RelayCommand]
        public async Task Initialize()
        {
            if (Students.Count > 0)
                return; // Already loaded

            try
            {
                IsLoading = true;
                ClearError();

                var students = await _dataService.GetMockStudentsAsync();
                Students = new ObservableCollection<Student>(students);
                FilteredStudents = new ObservableCollection<Student>(students);
            }
            catch (Exception ex)
            {
                SetError($"Failed to load students: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Applies the current filter to the student collection.
        /// </summary>
        [RelayCommand]
        public async Task ApplyFilter()
        {
            try
            {
                IsLoading = true;
                ClearError();

                await Task.Delay(200); // Simulate filtering delay

                var filtered = Students.AsEnumerable();

                // Filter by search term (name)
                if (!string.IsNullOrWhiteSpace(FilterTerm))
                {
                    var lowerSearch = FilterTerm.ToLower();
                    filtered = filtered.Where(s => s.Name.ToLower().Contains(lowerSearch));
                }

                // Filter by grade range
                filtered = filtered.Where(s => s.Grade >= MinGradeFilter && s.Grade <= MaxGradeFilter);

                FilteredStudents = new ObservableCollection<Student>(filtered);
            }
            catch (Exception ex)
            {
                SetError($"Failed to apply filter: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Clears all filters and resets to full student collection.
        /// </summary>
        [RelayCommand]
        public void ClearFilter()
        {
            // Ensure the entry control bound to FilterTerm is cleared
            FilterTerm = string.Empty;
            MinGradeFilter = 0;
            MaxGradeFilter = 100;
            FilteredStudents = new ObservableCollection<Student>(Students);
            ClearError();
        }

        /// <summary>
        /// Saves a grade update for a student.
        /// </summary>
        /// <param name="student">The student with updated grade.</param>
        [RelayCommand]
        public async Task SaveGrade(Student student)
        {
            try
            {
                var validationResult = _validationService.ValidateGrade(student.Grade);
                if (!validationResult.IsValid)
                {
                    SetError(validationResult.ErrorMessage);
                    return;
                }

                var updateResult = await _dataService.UpdateStudentAsync(student);
                if (!updateResult.IsValid)
                {
                    SetError(updateResult.ErrorMessage);
                    return;
                }

                ClearError();
                await Shell.Current.DisplayAlert("Success", "Grade saved successfully", "OK");
            }
            catch (Exception ex)
            {
                SetError($"Failed to save grade: {ex.Message}");
            }
        }

        /// <summary>
        /// Saves an attendance update for a student.
        /// </summary>
        /// <param name="student">The student with updated attendance.</param>
        [RelayCommand]
        public async Task SaveAttendance(Student student)
        {
            try
            {
                var validationResult = _validationService.ValidateAttendance(student.AttendancePercentage);
                if (!validationResult.IsValid)
                {
                    SetError(validationResult.ErrorMessage);
                    return;
                }

                var updateResult = await _dataService.UpdateStudentAsync(student);
                if (!updateResult.IsValid)
                {
                    SetError(updateResult.ErrorMessage);
                    return;
                }

                ClearError();
                await Shell.Current.DisplayAlert("Success", "Attendance saved successfully", "OK");
            }
            catch (Exception ex)
            {
                SetError($"Failed to save attendance: {ex.Message}");
            }
        }

        /// <summary>
        /// Navigates to the student detail page for editing.
        /// </summary>
        /// <param name="student">The student to view/edit.</param>
        [RelayCommand]
        public async Task NavigateToDetail(Student student)
        {
            if (student == null)
                return;

            try
            {
                await Shell.Current.GoToAsync($"student-detail?studentId={student.StudentId}");
            }
            catch (Exception ex)
            {
                SetError($"Navigation failed: {ex.Message}");
            }
        }
    }
}
