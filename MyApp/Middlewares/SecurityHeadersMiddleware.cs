namespace ApiApp.Middlewares
{
    public static class SecurityHeadersMiddlewareMiddlewareExtensions
    {
        public static IApplicationBuilder UseSecurityHeadersHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SecurityHeadersMiddleware>();
        }
    }

    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public SecurityHeadersMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task Invoke(HttpContext context)
        {
            var response = context.Response;

            var securityConfig = _configuration.GetSection("SecurityHeaders");

            // 1. X-Content-Type-Options
            var xContentType = securityConfig["XContentTypeOptions"];
            if (!string.IsNullOrEmpty(xContentType))
                response.Headers.Append("X-Content-Type-Options", xContentType);

            // 2. X-Frame-Options
            var xFrame = securityConfig["XFrameOptions"];
            if (!string.IsNullOrEmpty(xFrame))
                response.Headers.Append("X-Frame-Options", xFrame);

            // 3. HSTS
            var hsts = securityConfig.GetSection("StrictTransportSecurity");
            if (hsts.GetValue<bool>("Enabled"))
            {
                var hstsValue = $"max-age={hsts["MaxAge"]}";
                if (hsts.GetValue<bool>("IncludeSubDomains")) hstsValue += "; includeSubDomains";
                if (hsts.GetValue<bool>("Preload")) hstsValue += "; preload";
                response.Headers.Append("Strict-Transport-Security", hstsValue);
            }

            // 4. CSP
            var csp = securityConfig.GetSection("ContentSecurityPolicy");
            var cspValue = $"default-src {csp["DefaultSrc"]}; " +
                           $"script-src {csp["ScriptSrc"]}; " +
                           $"style-src {csp["StyleSrc"]}; " +
                           $"img-src {csp["ImgSrc"]}; " +
                           $"frame-ancestors {csp["FrameAncestors"]}; " +
                           $"object-src {csp["ObjectSrc"]};";
            response.Headers.Append("Content-Security-Policy", cspValue);

            // 5. Referrer-Policy
            var referrer = securityConfig["ReferrerPolicy"];
            if (!string.IsNullOrEmpty(referrer))
                response.Headers.Append("Referrer-Policy", referrer);

            await _next(context);
        }
    }
}
