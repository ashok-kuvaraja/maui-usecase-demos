using StudentGradesDashboard.Models;

namespace StudentGradesDashboard.Services
{
    /// <summary>
    /// Service interface for validating student data according to business rules.
    /// </summary>
    public interface IValidationService
    {
        /// <summary>
        /// Validates a grade value against business rules (0-100 range, 2 decimal places).
        /// </summary>
        /// <param name="grade">The grade value to validate.</param>
        /// <returns>A ValidationResult indicating success or failure with error details.</returns>
        ValidationResult ValidateGrade(decimal grade);

        /// <summary>
        /// Validates an attendance percentage against business rules (0-100 range, 0 decimal places).
        /// </summary>
        /// <param name="attendance">The attendance percentage to validate.</param>
        /// <returns>A ValidationResult indicating success or failure with error details.</returns>
        ValidationResult ValidateAttendance(decimal attendance);

        /// <summary>
        /// Validates a complete student record against all business rules.
        /// </summary>
        /// <param name="student">The student record to validate.</param>
        /// <returns>A ValidationResult indicating success or failure with error details.</returns>
        ValidationResult ValidateStudent(Student student);
    }
}
