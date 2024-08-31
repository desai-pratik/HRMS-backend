using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Hrms.Model; // Assuming Logs.ToJsonFile is defined here

namespace Hrms.Provider
{
    public static class LogsProvider
    {
        public static void WriteErrorLog(object exception, object data)
        {
            MethodBase methodBase = new StackFrame(1).GetMethod();
            WriteLog(new Logs()
            {
                Message = exception.ToJson(),
                Type = Convert.ToString(LogTypes.Error),
                Data = data != null ? data.ToJson() : string.Empty,
                Method = string.Format("{0}.{1}", methodBase.DeclaringType.FullName, methodBase.Name)
            });
        }

        private static void WriteLog(Logs logs)
        {
            string logsDBPath = Path.Combine(ConfigProvider.Provider.BaseDirectory, ConfigProvider.Settings.LogsFolderName, $"{DateTime.Today:ddMMyyyy}{ConfigProvider.Settings.LightDataExtension}");
            logs.ToJsonFile(logsDBPath);
        }

        private enum LogTypes : byte { Debug, Error, Info, Warning }

        public class Logs
        {
            [Key] public string Id { get; set; } = Guid.NewGuid().ToString();
            [Required] public DateTime Time { get; set; } = DateTime.Now.ToUniversalTime();
            [Required] public string Type { get; set; }
            public string Method { get; set; }
            public string Message { get; set; }
            public string Data { get; set; }
        }
    }

  
}
