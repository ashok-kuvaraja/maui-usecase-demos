using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using Microsoft.Maui.Storage;
using StudentGradesDashboard.Models;

namespace StudentGradesDashboard.Services
{
    /// <summary>
    /// Service for managing student data and mock data generation.
    /// Provides in-memory data access for the application.
    /// </summary>
    public class DataService : IDataService
    {
        private readonly List<Student> _students;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString,
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="DataService"/> class.
        /// Generates mock student data on initialization.
        /// </summary>
        public DataService()
        {
            _students = GenerateMockStudents();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Student>> GetMockStudentsAsync()
        {
            // Try to load a packaged JSON file first (Resources/Raw/students.json).
            // Support multiple JSON shapes: an array of students, or an object wrapper { "students": [...] }.
            try
            {
                using var stream = await FileSystem.OpenAppPackageFileAsync("students.json");
                using var ms = new System.IO.MemoryStream();
                await stream.CopyToAsync(ms);
                ms.Position = 0;

                List<Student>? result = null;

                // Try direct List<Student> shape
                try
                {
                    ms.Position = 0;
                    var direct = await JsonSerializer.DeserializeAsync<List<Student>>(ms, _jsonOptions);
                    if (direct != null && direct.Count > 0)
                        result = direct.ToList();
                }
                catch
                {
                    // ignore and attempt other shapes
                }

                // Try wrapper shape
                if (result == null)
                {
                    try
                    {
                        ms.Position = 0;
                        var wrapper = await JsonSerializer.DeserializeAsync<StudentWrapper>(ms, _jsonOptions);
                        if (wrapper?.Students != null && wrapper.Students.Count > 0)
                        {
                            var mapped = wrapper.Students
                                .Select(MapDtoToStudent)
                                .Where(s => s != null)
                                .Cast<Student>()
                                .ToList();

                            if (mapped.Count > 0)
                                result = mapped;
                        }
                    }
                    catch
                    {
                        // ignore and fall back
                    }
                }

                // If we have parsed JSON, ensure at least 1000 records by appending generated mock students
                if (result != null && result.Count > 0)
                {
                    const int target = 1000;

                    // Create a combined list starting with parsed results
                    var combined = result.ToList();

                    // If not enough records, generate additional mock students and append
                    if (combined.Count < target)
                    {
                        int needed = target - combined.Count;
                        int maxId = combined.Any() ? combined.Max(s => s.StudentId) : 0;

                        // Use GenerateMockStudents() as a filler source to avoid referencing the same collection
                        var filler = GenerateMockStudents();
                        foreach (var candidate in filler)
                        {
                            if (needed <= 0)
                                break;

                            maxId++;
                            var clone = new Student
                            {
                                StudentId = maxId,
                                Name = candidate.Name,
                                Class = candidate.Class,
                                Subject = candidate.Subject,
                                Grade = candidate.Grade,
                                AttendancePercentage = candidate.AttendancePercentage,
                                TeacherName = candidate.TeacherName,
                                AcademicYear = candidate.AcademicYear,
                                DateCreated = candidate.DateCreated,
                                DateModified = candidate.DateModified
                            };

                            combined.Add(clone);
                            needed--;
                        }
                    }

                    // Replace internal store so callers and update operations operate on the same instances
                    _students.Clear();
                    _students.AddRange(combined);

                    return _students.ToList();
                }
            }
            catch
            {
                // Not packaged or unreadable; fall back to in-memory mock data
            }

            // Simulate async operation for mock generation
            await Task.Delay(300);
            return _students.ToList();
        }

        private Student? MapDtoToStudent(StudentDto dto)
        {
            if (dto == null)
                return null;

            // pick id from common names
            var id = dto.StudentId ?? dto.Id ?? 0;

            // required fields check
            if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Class) || string.IsNullOrWhiteSpace(dto.Subject) || string.IsNullOrWhiteSpace(dto.TeacherName))
                return null;

            var grade = dto.Grade ?? 0m;
            var attendance = dto.AttendancePercentage ?? 0m;

            return new Student
            {
                StudentId = id,
                Name = dto.Name.Trim(),
                Class = dto.Class.Trim(),
                Subject = dto.Subject.Trim(),
                Grade = Math.Clamp(grade, 0m, 100m),
                AttendancePercentage = Math.Clamp(attendance, 0m, 100m),
                TeacherName = dto.TeacherName.Trim(),
                AcademicYear = dto.AcademicYear ?? DateTime.Now.Year,
                DateCreated = dto.DateCreated ?? DateTime.Now,
                DateModified = dto.DateModified ?? DateTime.Now
            };
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Student>> SearchByNameAsync(string searchTerm)
        {
            await Task.Delay(100);
            if (string.IsNullOrWhiteSpace(searchTerm))
                return _students.ToList();

            var lowerSearch = searchTerm.ToLower();
            return _students
                .Where(s => s.Name.ToLower().Contains(lowerSearch))
                .ToList();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Student>> FilterByClassAndSubjectAsync(string studentClass, string subject)
        {
            await Task.Delay(100);
            return _students
                .Where(s => s.Class == studentClass && s.Subject == subject)
                .ToList();
        }

        /// <inheritdoc/>
        public async Task<Student?> GetStudentByIdAsync(int studentId)
        {
            await Task.Delay(50);
            return _students.FirstOrDefault(s => s.StudentId == studentId);
        }

        /// <inheritdoc/>
        public async Task<ValidationResult> UpdateStudentAsync(Student student)
        {
            await Task.Delay(100);
            var existing = _students.FirstOrDefault(s => s.StudentId == student.StudentId);
            if (existing == null)
                return ValidationResult.Failure("Student not found");

            existing.Grade = student.Grade;
            existing.AttendancePercentage = student.AttendancePercentage;
            existing.DateModified = DateTime.Now;

            return ValidationResult.Success();
        }

        /// <summary>
        /// Generates mock student data (1000+ records) for testing and development.
        /// </summary>
        /// <returns>A list of mock student records.</returns>
        private List<Student> GenerateMockStudents()
        {
            var students = new List<Student>();
            var random = new Random(42); // Fixed seed for reproducibility

            var classes = new[] { "10A", "10B", "11A", "11B", "12A", "12B" };
            var subjects = new[] { "Mathematics", "Science", "English", "History", "Geography" };
            var firstNames = new[] { "Aditya", "Ananya", "Arjun", "Akshara", "Aman", "Anjali", "Aryan", "Ashita", "Abhishek", "Avni" };
            var lastNames = new[] { "Singh", "Kumar", "Sharma", "Patel", "Gupta", "Verma", "Iyer", "Nair", "Bansal", "Chopra" };
            var teacherNames = new[] { "Dr. Rajesh", "Mrs. Priya", "Mr. Vikram", "Ms. Deepika", "Prof. Arun", "Dr. Neha", "Mr. Sanjay", "Mrs. Amrita", "Dr. Hari", "Ms. Suniti" };

            int studentId = 1;

            for (int c = 0; c < classes.Length; c++)
            {
                for (int s = 0; s < subjects.Length; s++)
                {
                    for (int i = 0; i < 35; i++) // ~35 students per class-subject combination
                    {
                        var firstName = firstNames[random.Next(firstNames.Length)];
                        var lastName = lastNames[random.Next(lastNames.Length)];
                        var teacher = teacherNames[random.Next(teacherNames.Length)];

                        students.Add(new Student
                        {
                            StudentId = studentId++,
                            Name = $"{firstName} {lastName}",
                            Class = classes[c],
                            Subject = subjects[s],
                            Grade = (decimal)(random.NextDouble() * 100),
                            AttendancePercentage = (decimal)random.Next(60, 101),
                            TeacherName = teacher,
                            AcademicYear = 2024,
                            DateCreated = DateTime.Now.AddDays(-random.Next(1, 365)),
                            DateModified = DateTime.Now
                        });

                        if (studentId > 1050) // Stop at ~1050 students
                            goto Done;
                    }
                }
            }

            Done:
            return students;
        }
    }
}
