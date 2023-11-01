using System.IO;

namespace C_Sharp_LogHandler
{
    internal static class LogFileFinder
    {
        public static string[] FindLogFiles(string relativeFolderPath)
        {
            // var currentDirectory = Directory.GetCurrentDirectory();
            // /home/jd/scheduler/scheduler-cloud-logs
            var targetDirectory = "/home/jd/scheduler";
            // Console.WriteLine(currentDirectory);
            var folderPath = Path.Combine(targetDirectory, relativeFolderPath);

            //抓取當天日期的前一天
            var yesterday = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            
            // return yyyy-MM-dd.log files
            return Directory.GetFiles(folderPath, $"{yesterday}.log", SearchOption.AllDirectories);
            // return Directory.GetFiles(folderPath, "2023-09-24.log", SearchOption.AllDirectories);
        }
    }
}