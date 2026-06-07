using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;

namespace Core.Extensions
{
    public static class SecurityHelper
    {
        public static string GetSha256Hash(string input)
        {
            //using (var sha256 = new SHA256CryptoServiceProvider())
            using (var sha256 = SHA256.Create())
            {
                var byteValue = Encoding.UTF8.GetBytes(input);
                var byteHash = sha256.ComputeHash(byteValue);
                return Convert.ToBase64String(byteHash);
                //return BitConverter.ToString(byteHash).Replace("-", "").ToLower();
            }
        }
        public static string HashPassword(string password, string salt)
        {
            return Convert.ToBase64String(
                SHA256.Create()
                .ComputeHash(Encoding.UTF8.GetBytes(password + salt))
            );
        }

        public static string GenerateSalt()
        {
            return Convert.ToBase64String(
                SHA256.Create()
                .ComputeHash(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()))
            );
        }

        public static bool VerifyPassword(string password, string salt, string hash)
        {
            return HashPassword(password, salt) == hash;
        }

        public static int GenerateRandomCode(int _min = 10000, int _max = 99999)
        {
            Random _rdm = new Random();
            return _rdm.Next(_min, _max);
        }

        public static string GenerateKey(int byteKey)
        {
            byte[] keyBytes = new byte[byteKey];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(keyBytes);
            }
            string keyString = Convert.ToBase64String(keyBytes);
            return keyString;
        }

        public static DateTime GetTokenExpiry(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                return jwtToken.ValidTo;
            }
            catch
            {
                // اگر نتونست تاریخ رو بخونه، 15 دقیقه بعد در نظر بگیر
                return DateTime.UtcNow.AddMinutes(15);
            }
        }

        public static byte[] HexStringToByteArray(string hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }
    }
}
