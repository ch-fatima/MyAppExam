using Core.ApiResults;
using Core.Exceptions;
using Core.Extensions;
using Infrastructure.Logger;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Net;

namespace ApiApp.Middlewares
{
    public static class CustomExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomExceptionHandlerMiddleware>();
        }
    }

    public class CustomExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _env;
        private readonly ILoggerManager _logger;

        public CustomExceptionHandlerMiddleware(RequestDelegate next,
            IWebHostEnvironment env,
            ILoggerManager logger)
        {
            _next = next;
            _env = env;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            string message = null;
            HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError;
            ApiResultStatusCode apiStatusCode = ApiResultStatusCode.ServerError;

            var props = new UserRequestProperties
            {
                Url = context.Request.Path,
                Method = context.Request.Method,
                ClientIP = context.Connection.RemoteIpAddress?.ToString(),
                Host = context.Request.Host.ToString(),
                RegisterDate = DateTime.UtcNow.ToString("O"),
                Headers = JsonConvert.SerializeObject(context.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString())),
                CorrelationId = GetCorrelationId(context)
            };
            context.Items["UserRequestProps"] = props;
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                await _next(context);
                props.StatusCode = HttpStatusCode.OK;
                props.Status = ApiResultStatusCode.Success;
            }
            catch (AppException exception)
            {
                stopwatch.Stop();
                props.StatusCode = exception.HttpStatusCode;
                props.Status = exception.ApiStatusCode;
                props.Error = exception.Message;
                props.Duration = stopwatch.ElapsedMilliseconds;
                props.Description = $"AppException occurred. CorrelationId: {props.CorrelationId}";
                _logger.LogError(props);

                if (_env.IsDevelopment())
                {
                    var dic = new Dictionary<string, string>
                    {
                        ["Exception"] = exception.Message,
                        ["StackTrace"] = exception.StackTrace,
                    };
                    if (exception.InnerException != null)
                    {
                        dic.Add("InnerException.Exception", exception.InnerException.Message);
                        dic.Add("InnerException.StackTrace", exception.InnerException.StackTrace);
                    }
                    if (exception.AdditionalData != null)
                        dic.Add("AdditionalData", JsonConvert.SerializeObject(exception.AdditionalData));

                    message = JsonConvert.SerializeObject(dic);
                }
                else
                {

                    message = exception.Message;
                }
                await WriteToResponseAsync();
            }
            catch (SecurityTokenExpiredException exception)
            {
                stopwatch.Stop();
                props.Description = $"SecurityTokenExpiredException occurred. CorrelationId: {props.CorrelationId}";
                _logger.LogError(props);
                SetUnAuthorizeResponse(exception);
                props.Error = exception.Message;
                props.Duration = stopwatch.ElapsedMilliseconds;
                await WriteToResponseAsync();
            }
            catch (UnauthorizedAccessException exception)
            {
                stopwatch.Stop();
                props.Error = exception.Message;
                props.Duration = stopwatch.ElapsedMilliseconds;
                props.Description = $"UnauthorizedAccessException occurred. CorrelationId: {props.CorrelationId}";
                _logger.LogError(props);
                SetUnAuthorizeResponse(exception);
                await WriteToResponseAsync();
            }
            catch (Exception exception)
            {
                stopwatch.Stop();
                props.StatusCode = HttpStatusCode.InternalServerError;
                props.Status = ApiResultStatusCode.ServerError;
                props.Error = exception.Message;
                props.Duration = stopwatch.ElapsedMilliseconds;
                props.Description = $"Unhandled exception occurred. CorrelationId: {props.CorrelationId}";
                _logger.LogError(props);

                if (_env.IsDevelopment())
                {
                    var dic = new Dictionary<string, string>
                    {
                        ["Exception"] = exception.Message,
                        ["StackTrace"] = exception.StackTrace,
                    };
                    message = JsonConvert.SerializeObject(dic);
                }
                await WriteToResponseAsync();
            }

            async Task WriteToResponseAsync()
            {
                if (context.Response.HasStarted)
                    throw new InvalidOperationException("The response has already started, the http status code middleware will not be executed.");
                
                var result = new ApiResult(false, apiStatusCode, message);
                var json = MaskingHelper.MaskSensitiveText(JsonConvert.SerializeObject(result));

                context.Response.StatusCode = (int)httpStatusCode;
                context.Response.ContentType = "application/json";

                if (!string.IsNullOrWhiteSpace(props.CorrelationId))
                    context.Response.Headers["X-Correlation-ID"] = props.CorrelationId;

                await context.Response.WriteAsync(json);
            }

            void SetUnAuthorizeResponse(Exception exception)
            {
                httpStatusCode = HttpStatusCode.Unauthorized;
                apiStatusCode = ApiResultStatusCode.UnAuthorized;

                props.StatusCode = httpStatusCode;
                props.Status = apiStatusCode;

                if (_env.IsDevelopment())
                {
                    var dic = new Dictionary<string, string>
                    {
                        ["Exception"] = exception.Message,
                        ["StackTrace"] = exception.StackTrace
                    };
                    if (exception is SecurityTokenExpiredException tokenException)
                        dic.Add("Expires", tokenException.Expires.ToString());

                    message = JsonConvert.SerializeObject(dic);
                }
            }
        }

        private string GetCorrelationId(HttpContext context)
        {
            const string headerName = "X-Correlation-ID";

            if (context.Request.Headers.TryGetValue(headerName, out var correlationId) &&
                !string.IsNullOrWhiteSpace(correlationId))
            {
                return correlationId.ToString();
            }

            return context.TraceIdentifier;
        }
    }
}
