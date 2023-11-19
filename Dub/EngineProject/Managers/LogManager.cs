using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineProject.Managers
{
    public static class LogManager
    {
        public static void LogException(Exception ex, string message = "")
        {
            var folderName = SettingsManager.logFolderName;
            if (!Directory.Exists(folderName))
            {
                Directory.CreateDirectory(folderName);
            }
            var fileName = $"{DateTime.Now.ToString("dd_MM_yyyy")}.log";
            var filePath = Path.Combine(folderName, fileName);
            if (!File.Exists(filePath)) 
            {
                File.Create(filePath).Close();
            }
            var newLine = $"\r\n";
            var log = newLine + $"---------------------{DateTime.Now.ToString("HH:mm:ss")}---------------------";
            log += newLine + (string.IsNullOrWhiteSpace(message) ? "" : $"{message}: ") +
                ex.Message + (ex.InnerException == null ? "" : $"; inner: {ex.InnerException.Message}") +
                $"; stack: {ex.StackTrace}" +
                newLine;
            File.AppendAllText(filePath, log);
        }
    }
}
