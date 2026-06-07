namespace Infrastructure.Logger
{
    public interface ILoggerManager
    {
        void LogTrace(string message);
        void LogTrace(UserRequestProperties userRequest);

        void LogInfo(string message);
        void LogInfo(UserRequestProperties userRequest);

        void LogWarn(string message);
        void LogWarn(UserRequestProperties userRequest);

        void LogDebug(string message);
        void LogDebug(UserRequestProperties userRequest);

        void LogError(string message);
        void LogError(UserRequestProperties userRequest);

        void AddMiddleLog(string key, string json);

        void AddMiddleLogInConsole(ref UserRequestProperties userRequest, string key, string json);

        UserRequestProperties GetUserRequestProperties(ExternalServicesLogModel logModel);
    }
}
