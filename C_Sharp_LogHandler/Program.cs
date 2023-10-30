using System;

namespace C_Sharp_LogHandler
{
    internal static class Program
    {
        private static void Main()
        {
            var logFiles = LogFileFinder.FindLogFiles("scheduler-cloud-logs");
            var excelFileName = "errors.xlsx";

            ErrorExtractor.ExtractToExcel(logFiles, "errors.xlsx");

            Console.WriteLine("Done!");

            var jsonConfigPath = "emailConfig.json";
            var emailSubject = "Log Errors Report";
            var emailBody = "Attached is the log errors report generated on " + DateTime.Now;

            EmailSender.SendEmailWithAttachment(jsonConfigPath, emailSubject, emailBody, excelFileName);
            Console.WriteLine("Email sent!");
        }
    }
}