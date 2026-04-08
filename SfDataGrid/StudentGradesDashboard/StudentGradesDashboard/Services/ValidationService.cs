using StudentGradesDashboard.Models;

namespace StudentGradesDashboard.Services
{
    /// <summary>
    /// Service for validating student data according to business rules.
    /// </summary>
    public class ValidationService : IValidationService
    {
        /// <inheritdoc/>
        public ValidationResult ValidateGrade(decimal grade)
        {
            if (grade < 0 || grade > 100)
            {
                return ValidationResult.Failure("Grade must be between 0 and 100");
            }

            return ValidationResult.Success();
        }

        /// <inheritdoc/>
        public ValidationResult ValidateAttendance(decimal attendance)
        {
            if (attendance < 0 || attendance > 100)
            {
                return ValidationResult.Failure("Attendance must be between 0 and 100");
            }

            return ValidationResult.Success();
        }

        /// <inheritdoc/>
        public ValidationResult ValidateStudent(Student student)
        {
            var errors = new List<string>();

            // Validate StudentId
            if (student.StudentId <= 0)
            {
                errors.Add("StudentId must be greater than 0");
            }

            // Validate Name
            if (string.IsNullOrWhiteSpace(student.Name) || student.Name.Length > 100)
            {
                errors.Add("Name must be between 1 and 100 characters");
            }

            // Validate Class
            var validClasses = new[] { "10A", "10B", "11A", "11B", "12A", "12B" };
            if (!validClasses.Contains(student.Class))
            {
                errors.Add("Class must be one of: 10A, 10B, 11A, 11B, 12A, 12B");
            }

            // Validate Subject
            var validSubjects = new[] { "Mathematics", "Science", "English", "History", "Geography" };
            if (!validSubjects.Contains(student.Subject))
            {
                errors.Add("Subject must be one of: Mathematics, Science, English, History, Geography");
            }

            // Validate Grade
            var gradeValidation = ValidateGrade(student.Grade);
            if (!gradeValidation.IsValid)
            {
                errors.Add(gradeValidation.ErrorMessage!);
            }

            // Validate Attendance
            var attendanceValidation = ValidateAttendance(student.AttendancePercentage);
            if (!attendanceValidation.IsValid)
            {
                errors.Add(attendanceValidation.ErrorMessage!);
            }

            // Validate TeacherName
            if (string.IsNullOrWhiteSpace(student.TeacherName) || student.TeacherName.Length > 100)
            {
                errors.Add("TeacherName must be between 1 and 100 characters");
            }

            // Validate AcademicYear
            if (student.AcademicYear < 2020 || student.AcademicYear > 2030)
            {
                errors.Add("AcademicYear must be between 2020 and 2030");
            }

            if (errors.Count > 0)
            {
                return ValidationResult.Failure(errors);
            }

            return ValidationResult.Success();
        }
    }
}
