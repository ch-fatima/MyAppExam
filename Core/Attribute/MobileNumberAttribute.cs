using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Core.Attribute
{
    public class MobileNumberAttribute : ValidationAttribute
    {
        private static readonly Regex _mobileFormat = new Regex(
            @"^09\d{9}$",
            RegexOptions.Compiled
        );

        protected override ValidationResult IsValid(object value,
            ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success;
            if (value is not string mobile) return ValidationResult.Success;

            mobile = mobile.Replace(" ", "").Replace("-", "");

            if (!_mobileFormat.IsMatch(mobile))
                return new ValidationResult("Mobile Number Is Invalid");

            return ValidationResult.Success;
        }
    }
}
