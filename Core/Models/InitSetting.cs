using System.Collections.Generic;

namespace Core.Models
{
    public class InitSetting
    {
        public string LoggerKey { get; set; }
        public string DefaultuserRoleId { get; set; }
        public string BusinessKey { get; set; }
        public ConnectionStrings connectionStrings { get; set; }
        public virtual SettingToken Token { get; set; }
        public Cryptography Cryptography { get; set; }  
        public KestrelConfig kestrelConfig { get; set; }    
    }

    public class ConnectionStrings
    {
        public string DefaultConnection { get; set; }
    }
    public class SettingToken
    {
        public virtual SettingAccessToken AccessToken { get; set; }
        public SettingRefreshToken RefreshToken { get; set; }
    }
    public class SettingAccessToken
    {
        public string ExpirationInMinutes { get; set; }
        public virtual string Secret { get; set; }
    }
    public class SettingRefreshToken
    {
        public string ExpirationInMinutes { get; set; }
    }

    public class Cryptography
    {
        public string EncryptionKey { get; set; }
    }

    public class KestrelConfig
    {
        public EndpointsConfig Endpoints { get; set; }
        public CertificatesConfig Certificates { get; set; }
    }

    public class EndpointsConfig
    {
        public HttpEndpoint Http { get; set; }
        public HttpsEndpoint Https { get; set; }
    }

    public class HttpEndpoint
    {
        public string Url { get; set; }
    }

    public class HttpsEndpoint
    {
        public string Url { get; set; }
        public string Protocols { get; set; }
        public List<string> SslProtocols { get; set; }
    }

    public class CertificatesConfig
    {
        public DefaultCertificate Default { get; set; }
    }

    public class DefaultCertificate
    {
        public string Path { get; set; }
        public string Password { get; set; }
    }
}
