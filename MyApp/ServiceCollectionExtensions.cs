using ApiApp.Controllers;
using Core.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.OpenApi.Models;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Reflection;
using System.Security.Authentication;

namespace Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddSetConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var _setting = new InitSetting()
            {
                connectionStrings = new ConnectionStrings()
                {
                     DefaultConnection = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ConnectionStrings_DefaultConnection")) ? configuration["ConnectionStrings:DefaultConnection"] : Environment.GetEnvironmentVariable("ConnectionStrings_DefaultConnection"),

                },
                Token = new SettingToken()
                {
                    AccessToken = new SettingAccessToken()
                    {
                        ExpirationInMinutes = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("Token_AccessToken_ExpirationInMinutes")) ? configuration["Token:AccessToken:ExpirationInMinutes"] : Environment.GetEnvironmentVariable("Token_AccessToken_ExpirationInMinutes"),
                        Secret = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("Token_AccessToken_Secret")) ? configuration["Token:AccessToken:Secret"] : Environment.GetEnvironmentVariable("Token_AccessToken_Secret")
                    },

                    RefreshToken = new SettingRefreshToken()
                    {
                        ExpirationInMinutes = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("Token_RefreshToken_ExpirationInMinutes")) ? configuration["Token:RefreshToken:ExpirationInMinutes"] : Environment.GetEnvironmentVariable("Token_RefreshToken_ExpirationInMinutes"),                        
                    },
                },
                Cryptography = new Cryptography() { 
                     EncryptionKey = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("Cryptography_EncryptionKey")) ? configuration["Cryptography:EncryptionKey"] : Environment.GetEnvironmentVariable("Cryptography_EncryptionKey")
                },
                kestrelConfig = new KestrelConfig() { 
                     Certificates = new CertificatesConfig() { 
                           Default = new DefaultCertificate() { 
                                Password = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("Kestrel_Certificates_Password")) ? 
                                configuration["Kestrel:Certificates:Default:Password"] : Environment.GetEnvironmentVariable("Kestrel_Certificates_Password")
                           }
                     }
                },
                DefaultuserRoleId = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DefaultuserRoleId")) ? configuration["DefaultuserRoleId"] : Environment.GetEnvironmentVariable("DefaultuserRoleId"),
                BusinessKey = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("BusinessKey")) ? configuration["BusinessKey"] : Environment.GetEnvironmentVariable("BusinessKey")
            };
            
            services.AddSingleton<InitSetting>(_setting);
        }

        public static void AddDbConnection(this IServiceCollection services, string connectionString)
        {
            services.AddScoped<IDbConnection>((provider) => new SqlConnection(connectionString));
        }

        public static void AddMediatorConfig(this IServiceCollection services)
        {
            var apiAssembly = typeof(UserController).Assembly;
            var applicationAssembly = Assembly.Load("Application");
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(apiAssembly);
                cfg.RegisterServicesFromAssembly(applicationAssembly);
            });
        }

        public static void AddCorsConfig(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.SetIsOriginAllowed(_ => true)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });
        }

        public static IServiceCollection AddCustomKestrelConfiguration(
            this IServiceCollection services,
            IConfiguration configuration,
            IWebHostEnvironment env)
        {
            services.Configure<KestrelServerOptions>(options =>
            {
                options.ConfigurationLoader = null;

                var kestrelConfig = configuration.GetSection("Kestrel");

                // HTTP Endpoint
                var httpEndpoint = kestrelConfig.GetSection("Endpoints:Http");
                if (httpEndpoint.Exists())
                {
                    var httpUrl = httpEndpoint["Url"];
                    if (!string.IsNullOrEmpty(httpUrl))
                    {
                        var uri = new Uri(httpUrl);
                        var ipAddress = uri.Host == "localhost" ? IPAddress.Loopback : IPAddress.Any;
                        options.Listen(ipAddress, uri.Port);
                    }
                }

                // HTTPS Endpoint
                var httpsEndpoint = kestrelConfig.GetSection("Endpoints:Https");
                if (httpsEndpoint.Exists())
                {
                    var httpsUrl = httpsEndpoint["Url"];
                    if (!string.IsNullOrEmpty(httpsUrl))
                    {
                        var uri = new Uri(httpsUrl);
                        var ipAddress = uri.Host == "localhost" ? IPAddress.Loopback : IPAddress.Any;

                        options.Listen(ipAddress, uri.Port, listenOptions =>
                        {
                            if (httpsEndpoint["Protocols"] != null)
                            {
                                listenOptions.Protocols = Enum.Parse<HttpProtocols>(httpsEndpoint["Protocols"]);
                            }

                            listenOptions.UseHttps(httpsOptions =>
                            {
                                var sslProtocols = httpsEndpoint.GetSection("SslProtocols").Get<string[]>();
                                if (sslProtocols != null && sslProtocols.Any())
                                {
                                    var protocols = SslProtocols.None;
                                    foreach (var proto in sslProtocols)
                                    {
                                        protocols |= Enum.Parse<SslProtocols>(proto);
                                    }
                                    httpsOptions.SslProtocols = protocols;
                                }
                                else
                                {
                                    httpsOptions.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13;
                                }

                                httpsOptions.ClientCertificateMode = ClientCertificateMode.NoCertificate;
                                httpsOptions.CheckCertificateRevocation = !env.IsDevelopment();
                            });
                        });
                    }
                }

                // محدودیت‌ها
                var limits = kestrelConfig.GetSection("Limits");
                if (limits.Exists())
                {
                    if (limits["MaxConcurrentConnections"] != null)
                        options.Limits.MaxConcurrentConnections = limits.GetValue("MaxConcurrentConnections", 100);

                    if (limits["MaxRequestBodySize"] != null)
                        options.Limits.MaxRequestBodySize = limits.GetValue("MaxRequestBodySize", 10 * 1024 * 1024L);

                    if (limits["KeepAliveTimeoutSeconds"] != null)
                        options.Limits.KeepAliveTimeout = TimeSpan.FromSeconds(limits.GetValue("KeepAliveTimeoutSeconds", 120));
                }

                options.AllowSynchronousIO = false;
                options.AddServerHeader = false;
            });

            var hstsConfig = configuration.GetSection("Security:Hsts");

            services.AddHsts(options =>
            {
                // خواندن از کانفیگ با مقدار پیش‌فرض
                options.MaxAge = TimeSpan.FromDays(hstsConfig.GetValue("MaxAgeDays", 365));
                options.IncludeSubDomains = hstsConfig.GetValue("IncludeSubDomains", true);
                options.Preload = hstsConfig.GetValue("Preload", true);
                options.ExcludedHosts.Clear();

                // لاگ کردن مقدار تنظیم شده (برای دیباگ)
                var logger = services.BuildServiceProvider().GetService<ILogger<Program>>();
                logger?.LogInformation("HSTS configured: MaxAge={MaxAge} days, IncludeSubDomains={IncludeSubDomains}, Preload={Preload}",
                    options.MaxAge.TotalDays, options.IncludeSubDomains, options.Preload);
            });
            
            var httpsRedirectionConfig = configuration.GetSection("Security:HttpsRedirection");

            services.AddHttpsRedirection(options =>
            {
                // خواندن از کانفیگ با مقدار پیش‌فرض
                options.HttpsPort = httpsRedirectionConfig.GetValue("HttpsPort", 443);
                options.RedirectStatusCode = httpsRedirectionConfig.GetValue("RedirectStatusCode", StatusCodes.Status403Forbidden);

                // در محیط توسعه، از ریدایرکت موقت استفاده کن
                if (env.IsDevelopment() && !httpsRedirectionConfig.GetValue<bool>("OverrideForDevelopment", false))
                {
                    options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
                    options.HttpsPort = httpsRedirectionConfig.GetValue("HttpsPort", 7294);
                }

                // لاگ کردن مقدار تنظیم شده
                var logger = services.BuildServiceProvider().GetService<ILogger<Program>>();
                logger?.LogInformation("HTTPS Redirection configured: Port={Port}, StatusCode={StatusCode}",
                    options.HttpsPort, options.RedirectStatusCode);
            });

            return services;
        }
        public static void AddForwardedHeadersConfiguration(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
        {
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                // Forward کردن X-Forwarded-For و X-Forwarded-Proto
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

                // محدود کردن تعداد hop‌ها برای امنیت
                options.ForwardLimit = 1;

                // تنظیمات برای Production و Development
                if (!env.IsDevelopment())
                {
                    // در Production فقط به Proxy‌های مشخصی اعتماد کن
                    // اضافه کردن IP پروکسی‌های معتبر
                    var trustedProxies = configuration.GetSection("ForwardedHeaders:TrustedProxies").Get<string[]>();

                    if (trustedProxies != null && trustedProxies.Any())
                    {
                        foreach (var proxy in trustedProxies)
                        {
                            if (IPAddress.TryParse(proxy, out var ip))
                            {
                                options.KnownProxies.Add(ip);
                            }
                            else if (proxy.Contains('/'))
                            {
                                // برای subnet مثل 10.0.0.0/8
                                var parts = proxy.Split('/');
                                if (IPAddress.TryParse(parts[0], out var networkIp) && int.TryParse(parts[1], out var prefixLength))
                                {
                                    //options.KnownNetworks.Add(new IPNetwork(networkIp, prefixLength));
                                }
                            }
                        }
                    }

                    // اگر پروکسی خاصی مشخص نشده، حداقل لوپبک را اضافه کن
                    if (!options.KnownProxies.Any() && !options.KnownNetworks.Any())
                    {
                        options.KnownProxies.Add(IPAddress.Loopback);
                        options.KnownProxies.Add(IPAddress.IPv6Loopback);
                    }
                }
                else
                {
                    // در Development به همه پروکسی‌های لوپبک اعتماد کن
                    options.KnownProxies.Add(IPAddress.Loopback);
                    options.KnownProxies.Add(IPAddress.IPv6Loopback);
                }

                // رفع مشکل نبودن پورت در X-Forwarded-Host
                options.RequireHeaderSymmetry = false;
            });
        }
        public static void AddSwaggerConfig(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                var jwtSecurityScheme = new OpenApiSecurityScheme
                {
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Name = "JWT Authentication",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };
                c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { jwtSecurityScheme, Array.Empty<string>() }
                });
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Chourli v1 ExampleApi", Version = "v1" });
            });

        }
    }
}
