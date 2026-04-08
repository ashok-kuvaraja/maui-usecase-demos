using StudentGradesDashboard.Models;

namespace StudentGradesDashboard.Services
{
    /// <summary>
    /// Service interface for data access and student queries.
    /// </summary>
    public interface IDataService
    {
        /// <summary>
        /// Gets all mock students from the data source.
        /// </summary>
        /// <returns>A task representing the async operation that returns a collection of students.</returns>
        Task<IEnumerable<Student>> GetMockStudentsAsync();

        /// <summary>
        /// Searches for students by name.
        /// </summary>
        /// <param name="searchTerm">The search term to match against student names.</param>
        /// <returns>A task representing the async operation that returns matching students.</returns>
        Task<IEnumerable<Student>> SearchByNameAsync(string searchTerm);

        /// <summary>
        /// Filters students by class and subject.
        /// </summary>
        /// <param name="studentClass">The class to filter by.</param>
        /// <param name="subject">The subject to filter by.</param>
        /// <returns>A task representing the async operation that returns filtered students.</returns>
        Task<IEnumerable<Student>> FilterByClassAndSubjectAsync(string studentClass, string subject);

        /// <summary>
        /// Gets a single student by ID.
        /// </summary>
        /// <param name="studentId">The student ID.</param>
        /// <returns>A task representing the async operation that returns the student, or null if not found.</returns>
        Task<Student?> GetStudentByIdAsync(int studentId);

        /// <summary>
        /// Updates a student record.
        /// </summary>
        /// <param name="student">The student record to update.</param>
        /// <returns>A task representing the async operation that returns a ValidationResult.</returns>
        Task<ValidationResult> UpdateStudentAsync(Student student);
    }
}
