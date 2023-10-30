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
            var emailSubject = "Error通知";
            var emailBody = "log 如附件 Excel 檔";

            EmailSender.SendEmailWithAttachment(jsonConfigPath, emailSubject, emailBody, excelFileName);
            Console.WriteLine("Email sent!");
        }
    }
}