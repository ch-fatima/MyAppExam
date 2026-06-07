using System;
using System.Threading.Tasks;

namespace Infrastructure.TokenHelper
{
    public interface ITokenBlacklistHelper
    {
        Task AddToBlacklistAsync(string token, DateTime expiry);
        Task<bool> IsTokenBlacklistedAsync(string token);
    }
}
