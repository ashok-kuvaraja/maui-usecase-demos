using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StudentGradesDashboard.Models;
using StudentGradesDashboard.Services;

namespace StudentGradesDashboard.ViewModels
{
    /// <summary>
    /// ViewModel for the Student Detail page.
    /// Manages individual student editing and navigation.
    /// </summary>
    public partial class StudentDetailViewModel : BaseViewModel
    {
        private readonly IDataService _dataService;
        private readonly IValidationService _validationService;
        private Student? _originalStudent;

        /// <summary>
        /// Gets or sets the student ID (read-only, from route).
        /// </summary>
        [ObservableProperty]
        private int studentId;

        /// <summary>
        /// Gets or sets the student name (read-only).
        /// </summary>
        [ObservableProperty]
        private string? name;

        /// <summary>
        /// Gets or sets the student class (read-only).
        /// </summary>
        [ObservableProperty]
        private string? studentClass;

        /// <summary>
        /// Gets or sets the student subject (read-only).
        /// </summary>
        [ObservableProperty]
        private string? subject;

        /// <summary>
        /// Gets or sets the teacher name (read-only).
        /// </summary>
        [ObservableProperty]
        private string? teacherName;

        /// <summary>
        /// Gets or sets the student grade (editable).
        /// </summary>
        [ObservableProperty]
        private decimal grade;

        /// <summary>
        /// Gets or sets the student attendance percentage (editable).
        /// </summary>
        [ObservableProperty]
        private decimal attendancePercentage;

        /// <summary>
        /// Gets or sets the academic year for the student (read-only).
        /// </summary>
        [ObservableProperty]
        private int academicYear;

        /// <summary>
        /// Gets or sets a value indicating whether the form has unsaved changes.
        /// </summary>
        [ObservableProperty]
        private bool isModified;

        /// <summary>
        /// Initializes a new instance of the <see cref="StudentDetailViewModel"/> class.
        /// </summary>
        /// <param name="dataService">Service for data access.</param>
        /// <param name="validationService">Service for data validation.</param>
        public StudentDetailViewModel(IDataService dataService, IValidationService validationService)
        {
            _dataService = dataService;
            _validationService = validationService;
        }

        /// <summary>
        /// Initializes the view model by loading the student data.
        /// Extracts studentId from route query parameters.
        /// </summary>
        /// <param name="studentId">The student ID from navigation parameters.</param>
        [RelayCommand]
        public async Task Initialize(int? studentId = null)
        {
            try
            {
                IsLoading = true;
                ClearError();

                // Use passed parameter or existing StudentId
                if (studentId.HasValue && studentId.Value > 0)
                {
                    StudentId = studentId.Value;
                }

                // Get studentId from navigation parameters
                if (StudentId <= 0)
                {
                    SetError("Invalid student ID");
                    return;
                }

                var student = await _dataService.GetStudentByIdAsync(StudentId);
                if (student == null)
                {
                    SetError("Student not found");
                    return;
                }

                // Store original for comparison
                _originalStudent = student;

                // Load into editable fields
                Name = student.Name;
                StudentClass = student.Class;
                Subject = student.Subject;
                TeacherName = student.TeacherName;
                AcademicYear = student.AcademicYear;
                Grade = student.Grade;
                AttendancePercentage = student.AttendancePercentage;
                IsModified = false;
            }
            catch (Exception ex)
            {
                SetError($"Failed to load student: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Saves changes to the student record and navigates back.
        /// </summary>
        [RelayCommand]
        public async Task SaveChanges()
        {
            try
            {
                // Validate grade
                var gradeValidation = _validationService.ValidateGrade(Grade);
                if (!gradeValidation.IsValid)
                {
                    SetError($"Grade: {gradeValidation.ErrorMessage}");
                    return;
                }

                // Validate attendance
                var attendanceValidation = _validationService.ValidateAttendance(AttendancePercentage);
                if (!attendanceValidation.IsValid)
                {
                    SetError($"Attendance: {attendanceValidation.ErrorMessage}");
                    return;
                }

                if (_originalStudent == null)
                {
                    SetError("Student data not loaded");
                    return;
                }

                // Update the student
                _originalStudent.Grade = Grade;
                _originalStudent.AttendancePercentage = AttendancePercentage;

                var result = await _dataService.UpdateStudentAsync(_originalStudent);
                if (!result.IsValid)
                {
                    SetError(result.ErrorMessage);
                    return;
                }

                await Shell.Current.DisplayAlert("Success", "Student updated successfully", "OK");
                IsModified = false;

                // Navigate back
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                SetError($"Failed to save changes: {ex.Message}");
            }
        }

        /// <summary>
        /// Discards changes and navigates back to the previous page.
        /// </summary>
        [RelayCommand]
        public async Task Cancel()
        {
            IsModified = false;
            ClearError();
            await Shell.Current.GoToAsync("..");
        }

        /// <summary>
        /// Called when the Grade property changes to mark form as modified.
        /// </summary>
        partial void OnGradeChanged(decimal value)
        {
            IsModified = _originalStudent != null && 
                        (Grade != _originalStudent.Grade || 
                         AttendancePercentage != _originalStudent.AttendancePercentage);
        }

        /// <summary>
        /// Called when the AttendancePercentage property changes to mark form as modified.
        /// </summary>
        partial void OnAttendancePercentageChanged(decimal value)
        {
            IsModified = _originalStudent != null && 
                        (Grade != _originalStudent.Grade || 
                         AttendancePercentage != _originalStudent.AttendancePercentage);
        }
    }
}
