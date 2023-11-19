using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineProject.Managers
{
    public static class SettingsManager
    {
        public static int sectorsCount = 9;
        public static string path = ""; //@"C:\temp\images\тест";
        public static List<string> extentions = new List<string>() { ".png", ".jpg", ".gif", ".bmp" };
        public static double maxDupDifferenceInProcent = 0.05d;
        public static string dupFolderName = "Duplicates";
        public static string logFolderName = "Logs";
        public static int maxWidth = 640;
        public static int maxHeight = 480;
    }
}
