using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Core.Attribute
{
    public class SanitizedTextAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            string input = value.ToString();

            // Remove any HTML tags
            string sanitized = Regex.Replace(input, @"<[^>]*>", string.Empty);

            // Remove script-like patterns
            sanitized = Regex.Replace(sanitized, @"javascript\s*:", string.Empty, RegexOptions.IgnoreCase);
            sanitized = Regex.Replace(sanitized, @"on\w+\s*=", string.Empty, RegexOptions.IgnoreCase);

            // Check for encoded attacks
            if (sanitized != input)
            {
                return new ValidationResult("HTML/script tags or dangerous patterns are not allowed");
            }

            // Whitelist for safe characters in text fields
            if (!Regex.IsMatch(input, @"^[a-zA-Z0-9\s\.\,\!\?\-\'\""\n\r\t]+$"))
            {
                return new ValidationResult("Text contains invalid characters. Only alphanumeric, basic punctuation and spaces allowed");
            }

            return ValidationResult.Success;
        }
    }
}
