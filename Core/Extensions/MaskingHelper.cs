using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Core.Extensions
{
    public static class MaskingHelper
    {
        private static readonly HashSet<string> SensitiveKeys = new(StringComparer.OrdinalIgnoreCase)
        {
            "Authorization",
            "X-Api-Key",
            "NationalCode",
            "Password",
            "Token",
            "AccountNumberCode"
        };

        public static string MaskSensitiveText(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            var maskedText = input;

            foreach (var key in SensitiveKeys)
            {
                var pattern = $@"({Regex.Escape(key)}\s*[:=]\s*)([^\s,;""']+)";

                maskedText = Regex.Replace(
                    maskedText,
                    pattern,
                    match => match.Groups[1].Value + MaskValue(match.Groups[2].Value),
                    RegexOptions.IgnoreCase
                );
            }

            return maskedText;
        }

        private static string MaskValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return "****";
            if (value.Length <= 4) return "****";

            return "****" + value[^4..];
        }
    }
}
