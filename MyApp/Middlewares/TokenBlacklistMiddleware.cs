using Infrastructure.Logger;
using Infrastructure.TokenHelper;

namespace ApiApp.Middlewares
{
    public static class TokenBlacklistMiddlewareExtensions
    {
        public static IApplicationBuilder UseBlacklistTokenHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TokenBlacklistMiddleware>();
        }
    }
    public class TokenBlacklistMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILoggerManager _logger;

        public TokenBlacklistMiddleware(RequestDelegate next, ILoggerManager logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, ITokenBlacklistHelper blacklistService)
        {
            // مسیرهایی که نیاز به چک ندارند
            var path = context.Request.Path.Value?.ToLower();
            var excludePaths = new[]
            {
            "/api/auth/login",
            "/swagger"
            };

            if (excludePaths.Any(p => path?.StartsWith(p) == true))
            {
                await _next(context);
                return;
            }

            // گرفتن Access Token
            var token = GetAccessToken(context);

            if (!string.IsNullOrEmpty(token))
            {
                // چک کردن در Blacklist
                var isBlacklisted = await blacklistService.IsTokenBlacklistedAsync(token);

                if (isBlacklisted)
                {
                    _logger.LogWarn($"Blocked request from blacklisted token. Path: {path}");

                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";

                    var response = new
                    {
                        status = 401,
                        message = "توکن شما نامعتبر شده است. لطفاً دوباره وارد شوید.",
                        error = "Token revoked"
                    };

                    await context.Response.WriteAsJsonAsync(response);
                    return;
                }
            }

            await _next(context);
        }

        private string GetAccessToken(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (string.IsNullOrEmpty(token))
            {
                token = context.Request.Cookies["accessToken"];
            }

            return token;
        }
    }
}
