using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Core.Attribute
{
    public static class DateValidator
    {
        public static ValidationResult ValidateDate(string date)
        {
            if (string.IsNullOrEmpty(date))
                return ValidationResult.Success;

            if (!DateTime.TryParseExact(date, "yyyy/MM/dd",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
                return new ValidationResult("Date Is Invalid");
            }

            if (parsedDate > DateTime.Now)
            {
                return new ValidationResult("تاریخ نمی‌تواند در آینده باشد");
            }

            return ValidationResult.Success;
        }
    }
}
