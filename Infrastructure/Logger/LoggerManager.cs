using Core.Extensions;
using Core.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Logger
{
    public class LoggerManager : ILoggerManager
    {
        private readonly ILogger _logger;
        private readonly IHttpContextAccessor _accessor;
        private readonly InitSetting _initSetting;

        public LoggerManager(IHttpContextAccessor accessor, InitSetting initSetting, ILogger logger = null)
        {
            _accessor = accessor;
            _initSetting = initSetting;
            _logger = logger ?? LogManager.GetCurrentClassLogger();
        }

        #region Log Methods with string message (Masked)

        public void LogDebug(string message)
        {
            try
            {
                _logger.Debug(MaskingHelper.MaskSensitiveText(message));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void LogTrace(string message)
        {
            try
            {
                _logger.Trace(MaskingHelper.MaskSensitiveText(message));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void LogInfo(string message)
        {
            try
            {
                _logger.Info(MaskingHelper.MaskSensitiveText(message));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void LogWarn(string message)
        {
            try
            {
                _logger.Warn(MaskingHelper.MaskSensitiveText(message));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void LogError(string message)
        {
            try
            {
                _logger.Error(MaskingHelper.MaskSensitiveText(message));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        #endregion

        #region Log Methods with UserRequestProperties

        public void LogDebug(UserRequestProperties userRequest)
        {
            try
            {
                _logger.Debug(GetLogEventInfo(LogLevel.Debug, userRequest));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void LogTrace(UserRequestProperties userRequest)
        {
            try
            {
                _logger.Trace(GetLogEventInfo(LogLevel.Trace, userRequest));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void LogInfo(UserRequestProperties userRequest)
        {
            try
            {
                _logger.Info(GetLogEventInfo(LogLevel.Info, userRequest));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void LogWarn(UserRequestProperties userRequest)
        {
            try
            {
                _logger.Warn(GetLogEventInfo(LogLevel.Warn, userRequest));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void LogError(UserRequestProperties userRequest)
        {
            try
            {
                _logger.Error(GetLogEventInfo(LogLevel.Error, userRequest));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        #endregion

        #region Private Methods

        private LogEventInfo GetLogEventInfo(LogLevel logLevel, UserRequestProperties userRequest)
        {
            var logEventInfo = new LogEventInfo(logLevel, "UserRequest", userRequest.Url);

            // Mask sensitive data in Params and Response
            userRequest.Params = MaskAndFormatJson(userRequest.Params);
            userRequest.Response = MaskAndFormatJson(userRequest.Response);
            userRequest.Error = MaskingHelper.MaskSensitiveText(userRequest.Error);

            // Set default values
            userRequest.ServiceType ??= EServiceType.Internal.ToString();

            // Add properties to log event
            logEventInfo.Properties.Add("Code", userRequest.Code);
            logEventInfo.Properties.Add("Url", userRequest.Url);
            logEventInfo.Properties.Add("Headers", MaskingHelper.MaskSensitiveText(userRequest.Headers?.Replace("\n", "")));
            logEventInfo.Properties.Add("TransactionID", userRequest.TransactionID);
            logEventInfo.Properties.Add("TransactionDate", userRequest.TransactionDate);
            logEventInfo.Properties.Add("RegisterDate", userRequest.RegisterDate);
            logEventInfo.Properties.Add("Params", userRequest.Params);
            logEventInfo.Properties.Add("Responce", userRequest.Response);
            logEventInfo.Properties.Add("Duration", userRequest.Duration);
            logEventInfo.Properties.Add("Status", userRequest.Status);
            logEventInfo.Properties.Add("ClientIP", userRequest.ClientIP);
            logEventInfo.Properties.Add("Error", string.IsNullOrEmpty(userRequest.Error) ? "(:" : userRequest.Error);
            logEventInfo.Properties.Add("Host", userRequest.Host);
            logEventInfo.Properties.Add("MiddleLog", userRequest.MiddleLog + Environment.NewLine);
            logEventInfo.Properties.Add("NationalCode", MaskingHelper.MaskSensitiveText(userRequest.NationalCode));
            logEventInfo.Properties.Add("MobileNumber", MaskingHelper.MaskSensitiveText(userRequest.MobileNumber));
            logEventInfo.Properties.Add("ServiceType", userRequest.ServiceType);
            logEventInfo.Properties.Add("StatusCode", userRequest.StatusCode);

            // Generate and add hash
            userRequest.Hash = HashData(logEventInfo.Properties);
            logEventInfo.Properties.Add("Hash", userRequest.Hash);

            return logEventInfo;
        }

        private string MaskAndFormatJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return json;

            try
            {
                var formattedJson = JToken.Parse(json).ToString(Formatting.Indented);
                return MaskingHelper.MaskSensitiveText(formattedJson);
            }
            catch
            {
                return MaskingHelper.MaskSensitiveText(json);
            }
        }

        private string HashData(IDictionary<object, object> properties)
        {
            string propertiesJson = JsonConvert.SerializeObject(properties);
            byte[] propertiesBytes = Encoding.UTF8.GetBytes(propertiesJson);
            byte[] keyBytes = Encoding.UTF8.GetBytes(_initSetting.LoggerKey);

            using var hmac = new HMACSHA256(keyBytes);
            byte[] hashBytes = hmac.ComputeHash(propertiesBytes);

            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }

        #endregion

        #region Public Methods

        public void AddMiddleLog(string key, string json)
        {
            string text = FormatAndMaskJson(json);

            try
            {
                if (_accessor.HttpContext?.Items["UserRequestlog"] is UserRequestProperties userRequest)
                {
                    userRequest.MiddleLog += $"+ {key} : {text} \n";
                    _accessor.HttpContext.Items["UserRequestlog"] = userRequest;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void AddMiddleLogInConsole(ref UserRequestProperties userRequest, string key, string json)
        {
            string text = FormatAndMaskJson(json);
            userRequest.MiddleLog += $"+ {key} : {text}  \n";
        }

        private string FormatAndMaskJson(string json)
        {
            try
            {
                string formatted = JToken.Parse(json).ToString(Formatting.Indented);
                return MaskingHelper.MaskSensitiveText(formatted);
            }
            catch
            {
                return MaskingHelper.MaskSensitiveText(json);
            }
        }

        public UserRequestProperties GetUserRequestProperties(ExternalServicesLogModel logModel)
        {
            var userRequestProperties = new UserRequestProperties
            {
                Code = Guid.NewGuid().ToString(),
                RegisterDate = DateTime.Now.ToString(),
                ServiceType = logModel?.ServiceType?.ToString(),
                ClientIP = _accessor.HttpContext?.Connection?.RemoteIpAddress?.ToString(),
                Duration = logModel?.Duration,
                Url = logModel?.Url
            };

            userRequestProperties.Params = SerializeAndMask(logModel?.Params);
            userRequestProperties.Response = SerializeAndMask(logModel?.Response);
            userRequestProperties.Error = SerializeAndMask(logModel?.Error) ?? logModel?.Error?.ToString();

            return userRequestProperties;
        }

        private string SerializeAndMask(object data)
        {
            if (data == null) return null;

            try
            {
                var json = JsonConvert.SerializeObject(data, Formatting.None);
                return MaskingHelper.MaskSensitiveText(json);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}