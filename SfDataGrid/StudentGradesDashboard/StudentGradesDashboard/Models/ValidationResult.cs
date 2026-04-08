namespace StudentGradesDashboard.Models
{
    /// <summary>
    /// Represents the result of a validation operation.
    /// </summary>
    public class ValidationResult
    {
        /// <summary>
        /// Gets or sets a value indicating whether the validation passed.
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Gets or sets a single error message (used when there is one primary error).
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets a collection of error messages (for validation with multiple errors).
        /// </summary>
        public ICollection<string> ErrorMessages { get; set; } = new List<string>();

        /// <summary>
        /// Creates a successful validation result.
        /// </summary>
        /// <returns>A ValidationResult indicating success.</returns>
        public static ValidationResult Success() => new() { IsValid = true };

        /// <summary>
        /// Creates a failed validation result with a single error message.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <returns>A ValidationResult indicating failure.</returns>
        public static ValidationResult Failure(string errorMessage) =>
            new() { IsValid = false, ErrorMessage = errorMessage };

        /// <summary>
        /// Creates a failed validation result with multiple error messages.
        /// </summary>
        /// <param name="errorMessages">The collection of error messages.</param>
        /// <returns>A ValidationResult indicating failure.</returns>
        public static ValidationResult Failure(ICollection<string> errorMessages) =>
            new() { IsValid = false, ErrorMessages = errorMessages };
    }
}
