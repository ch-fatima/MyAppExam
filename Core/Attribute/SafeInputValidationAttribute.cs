using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Core.Attribute
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public class SafeInputValidationAttribute : ValidationAttribute
    {
        // (Whitelist)
        private static readonly Regex _allowedPattern = new Regex(
            @"^[a-zA-Z0-9\s\-_.،?!@#$%&*()+=:;،\u0600-\u06FF\u0750-\u077F\u08A0-\u08FF\uFB50-\uFDFF\uFE70-\uFEFF]+$",
            RegexOptions.Compiled);

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || value is not string stringValue)
                return ValidationResult.Success;

            if (!_allowedPattern.IsMatch(stringValue))
            {
                return new ValidationResult(
                    $"Input contains invalid characters.",
                    new[] { validationContext.DisplayName }
                );
            }

            return ValidationResult.Success;
        }
    }
}
