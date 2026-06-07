using Infrastructure.Logger;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.TokenHelper
{
    public class TokenBlacklistHelper : ITokenBlacklistHelper
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILoggerManager _logger;

        public TokenBlacklistHelper(IMemoryCache memoryCache, ILoggerManager logger)
        {
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public async Task AddToBlacklistAsync(string token, DateTime expiry)
        {
            try
            {
                // هش کردن توکن برای امنیت بیشتر
                var tokenHash = ComputeHash(token);
                var cacheKey = $"bl_{tokenHash}";

                // محاسبه زمان باقی مانده تا انقضای توکن
                var ttl = expiry - DateTime.UtcNow;

                if (ttl > TimeSpan.Zero)
                {
                    // ذخیره در کش (فوری و سریع)
                    _memoryCache.Set(cacheKey, true, ttl);

                    _logger.LogInfo($"Token added to blacklist cache, expires in {ttl.TotalMinutes} minutes");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding token to blacklist: {ex.Message}");
            }

        }

        public async Task<bool> IsTokenBlacklistedAsync(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                    return false;

                var tokenHash = ComputeHash(token);
                var cacheKey = $"bl_{tokenHash}";

                // اول چک در کش (سریع)
                if (_memoryCache.TryGetValue(cacheKey, out bool isBlacklisted))
                {
                    return isBlacklisted;
                }

                // (اختیاری) اگر در کش نبود، در دیتابیس چک کن
                // var dbToken = await _unitOfWork.BlacklistedTokenRepository.GetByTokenHashAsync(tokenHash);
                // if (dbToken != null && dbToken.ExpiryDate > DateTime.UtcNow)
                // {
                //     // برگردوندن به کش
                //     var ttl = dbToken.ExpiryDate - DateTime.UtcNow;
                //     _memoryCache.Set(cacheKey, true, ttl);
                //     return true;
                // }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error checking token blacklist: {ex.Message}");
                return false;
            }
        }

        private string ComputeHash(string token)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(token);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
