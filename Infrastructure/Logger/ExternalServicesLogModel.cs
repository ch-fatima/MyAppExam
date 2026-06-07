using System;

namespace Infrastructure.Logger
{
    public class ExternalServicesLogModel
    {
        public EServiceType? ServiceType { get; set; }
        public object Params { get; set; }
        public object Response { get; set; } = string.Empty;
        public long? Duration { get; set; }
        public string Url { get; set; } = string.Empty;
        public Exception Error { get; set; } = new Exception();
    }

}
