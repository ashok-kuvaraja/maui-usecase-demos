using System.Text.Json.Serialization;

namespace StudentGradesDashboard.Services
{
    // Wrapper for JSON shapes like { "students": [ ... ] }
    internal class StudentWrapper
    {
        [JsonPropertyName("students")]
        public List<StudentDto>? Students { get; set; }

        [JsonPropertyName("count")]
        public int? Count { get; set; }
    }

    // DTO matching common JSON field names; fields are lenient to allow string numbers
    internal class StudentDto
    {
        [JsonPropertyName("studentId")]
        public int? StudentId { get; set; }

        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("class")]
        public string? Class { get; set; }

        [JsonPropertyName("subject")]
        public string? Subject { get; set; }

        [JsonPropertyName("grade")]
        public decimal? Grade { get; set; }

        [JsonPropertyName("attendancePercentage")]
        public decimal? AttendancePercentage { get; set; }

        [JsonPropertyName("teacherName")]
        public string? TeacherName { get; set; }

        [JsonPropertyName("academicYear")]
        public int? AcademicYear { get; set; }

        [JsonPropertyName("dateCreated")]
        public DateTime? DateCreated { get; set; }

        [JsonPropertyName("dateModified")]
        public DateTime? DateModified { get; set; }
    }
}
