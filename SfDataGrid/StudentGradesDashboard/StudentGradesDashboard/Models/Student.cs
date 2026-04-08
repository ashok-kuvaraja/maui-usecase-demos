using CommunityToolkit.Mvvm.ComponentModel;

namespace StudentGradesDashboard.Models
{
    /// <summary>
    /// Represents a student in the system with academic records.
    /// Implements change notification so UI bound to properties updates correctly.
    /// </summary>
    public class Student : ObservableObject
    {
        private int _studentId;
        /// <summary>
        /// Gets or sets the unique student identifier.
        /// </summary>
        public int StudentId
        {
            get => _studentId;
            set => SetProperty(ref _studentId, value);
        }

        private string _name = string.Empty;
        /// <summary>
        /// Gets or sets the student's full name.
        /// </summary>
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private string _class = string.Empty;
        /// <summary>
        /// Gets or sets the student's class/section (e.g., "10A", "11B").
        /// </summary>
        public string Class
        {
            get => _class;
            set => SetProperty(ref _class, value);
        }

        private string _subject = string.Empty;
        /// <summary>
        /// Gets or sets the subject (e.g., "Mathematics", "Science").
        /// </summary>
        public string Subject
        {
            get => _subject;
            set => SetProperty(ref _subject, value);
        }

        private decimal _grade;
        /// <summary>
        /// Gets or sets the numeric grade (0-100, up to 2 decimal places).
        /// Editable field.
        /// </summary>
        public decimal Grade
        {
            get => _grade;
            set => SetProperty(ref _grade, value);
        }

        private decimal _attendancePercentage;
        /// <summary>
        /// Gets or sets the attendance percentage (0-100, no decimals).
        /// Editable field.
        /// </summary>
        public decimal AttendancePercentage
        {
            get => _attendancePercentage;
            set => SetProperty(ref _attendancePercentage, value);
        }

        private string _teacherName = string.Empty;
        /// <summary>
        /// Gets or sets the name of the teacher for this subject.
        /// </summary>
        public string TeacherName
        {
            get => _teacherName;
            set => SetProperty(ref _teacherName, value);
        }

        private int _academicYear;
        /// <summary>
        /// Gets or sets the academic year (e.g., 2024, 2025).
        /// </summary>
        public int AcademicYear
        {
            get => _academicYear;
            set => SetProperty(ref _academicYear, value);
        }

        private DateTime _dateCreated;
        /// <summary>
        /// Gets or sets the date when the record was created.
        /// </summary>
        public DateTime DateCreated
        {
            get => _dateCreated;
            set => SetProperty(ref _dateCreated, value);
        }

        private DateTime? _dateModified;
        /// <summary>
        /// Gets or sets the date when the record was last modified.
        /// </summary>
        public DateTime? DateModified
        {
            get => _dateModified;
            set => SetProperty(ref _dateModified, value);
        }
    }
}
