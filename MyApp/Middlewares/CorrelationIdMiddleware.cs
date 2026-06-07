namespace ApiApp.Middlewares
{
    public static class CustomCorrelationHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseCorrelationIdHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CorrelationIdMiddleware>();
        }
    }

    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;
        private const string CorrelationIdHeaderKey = "X-Correlation-ID";

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context, ILogger<CorrelationIdMiddleware> logger)
        {
            if (!context.Request.Headers.TryGetValue(CorrelationIdHeaderKey, out var correlationId))
            {
                correlationId = Guid.NewGuid().ToString();
            }

            context.Response.Headers[CorrelationIdHeaderKey] = correlationId;
            using (logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId.ToString() }))
            {
                await _next(context);
            }
        }
    }
}
