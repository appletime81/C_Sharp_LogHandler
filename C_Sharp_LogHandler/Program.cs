using System;

namespace C_Sharp_LogHandler
{
    internal static class Program
    {
        private static void Main()
        {
            var logFiles = LogFileFinder.FindLogFiles("scheduler-cloud-logs");
            var excelFileName = "errors.xlsx";

            // extract errors to excel
            ErrorExtractor.ExtractToExcel(logFiles, "errors.xlsx");
            Console.WriteLine("errors.xlsx created!");
            
            // generate stats result from excel
            var errorStats = StatisticsCount.ReadExcel(excelFileName);
            Console.WriteLine("errors.xlsx read!");
            
            // log config file
            var jsonConfigPath = "emailConfig.json";
            
            // get current date - 1 day
            var dateStr = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            
            // named email subject
            var emailSubject = $"Error(Data from {dateStr}.log)";
            
            // get html(stats report) content
            var emailBody = StatisticsCount.GenerateHTMLFromStats(errorStats);
            
            // send email
            EmailSender.SendEmailWithAttachment(jsonConfigPath, emailSubject, emailBody, excelFileName);
            Console.WriteLine("Email sent!");
        }
    }
}