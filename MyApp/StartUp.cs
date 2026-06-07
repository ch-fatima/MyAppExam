using ApiApp.Middlewares;
using Application;
using Core.Extensions;
using Core.Models;
using Infrastructure;
using NLog;
using wallet.lib.BaseApi.Jwt;
namespace ApiApp
{
    public class StartUp
    {
        public IConfiguration Configuration { get; }
        private InitSetting _setting;
        public IWebHostEnvironment _env { get; }
        private readonly string nlogFile;

        public StartUp(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            _env = environment;
            nlogFile = "NLog.config";
            LogManager.LoadConfiguration(nlogFile);
            _setting = configuration.GetSection(nameof(InitSetting)).Get<InitSetting>();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSetConfiguration(Configuration);
            _setting = services.BuildServiceProvider().GetRequiredService<InitSetting>();
            #region --Kestrel&Hsts--
            services.AddCustomKestrelConfiguration(Configuration, _env);
            #endregion
            services.AddForwardedHeadersConfiguration(Configuration, _env);
            services.AddDbConnection(_setting.connectionStrings.DefaultConnection);
            services.AddJwtAuthorization(_setting.Token.AccessToken.Secret);
            services.AddCorsConfig();
            services.AddInfrastructureDI();
            services.AddApplicationDI();
            services.AddControllers().ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = false;
            });
            services.AddMediatorConfig();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerConfig();
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.Use(async (context, next) =>
            {
                var isHttps = context.Request.IsHttps;
                var forwardedProto = context.Request.Headers["X-Forwarded-Proto"].ToString();

                if (!isHttps && !forwardedProto.Equals("https", StringComparison.OrdinalIgnoreCase))
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Response.ContentType = "application/json";

                    var errorResponse = new
                    {
                        error = "HTTPS (TLS) is required",
                        status = 403,
                        code = "HTTPS_REQUIRED",
                        message = "This API only accepts secure HTTPS connections"
                    };

                    await context.Response.WriteAsync(
                        System.Text.Json.JsonSerializer.Serialize(errorResponse));
                    return; 
                }

                if (!env.IsDevelopment())
                {
                    context.Response.Headers.Append("Strict-Transport-Security",
                        "max-age=31536000; includeSubDomains; preload");
                }
                await next(); 
            });

            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            if (!env.IsDevelopment())
            {
                app.UseHsts(); 
            }

            app.UseCors("CorsPolicy");
            app.UseExceptionHandler("/Error");
            app.UseSecurityHeadersHandler();
            app.UseBlacklistTokenHandler();
            app.UseCorrelationIdHandler();
            app.UseCustomExceptionHandler();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            app.Use((context, next) =>
            {
                context.Request.PathBase = "";
                return next();
            });
        }
    }
}
