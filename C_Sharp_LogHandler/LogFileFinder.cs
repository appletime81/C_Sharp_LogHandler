using System.IO;

namespace C_Sharp_LogHandler
{
    internal static class LogFileFinder
    {
        public static string[] FindLogFiles(string relativeFolderPath)
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var folderPath = Path.Combine(currentDirectory, relativeFolderPath);

            //抓取當天日期的前一天
            var yesterday = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            
            // return yyyy-MM-dd.log files
            return Directory.GetFiles(folderPath, $"{yesterday}.log", SearchOption.AllDirectories);
        }
    }
}