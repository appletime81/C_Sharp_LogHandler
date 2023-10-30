using System.IO;
using OfficeOpenXml;

namespace C_Sharp_LogHandler
{
    internal static class ErrorExtractor
    {
        public static void ExtractToExcel(string[] logFiles, string relativeExcelPath)
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var excelFilePath = Path.Combine(currentDirectory, relativeExcelPath);

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Errors");

            // 設置Column名稱
            worksheet.Cells[1, 1].Value = "File Name";
            worksheet.Cells[1, 2].Value = "Error Message";

            int rowIndex = 2; // 因為第一行已被Column名稱佔用，所以從第二行開始寫入資料
            int maxFileNameLength = "File Name".Length; // 初始化為Column名稱的長度
            int maxErrorLength = "Error Message".Length; // 初始化為Column名稱的長度

            foreach (var file in logFiles)
            {
                using var reader = new StreamReader(file);
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Contains("ERROR"))
                    {
                        maxFileNameLength = Math.Max(maxFileNameLength, file.Length);
                        maxErrorLength = Math.Max(maxErrorLength, line.Length);

                        worksheet.Cells[rowIndex, 1].Value = file; // 記錄檔名
                        worksheet.Cells[rowIndex, 2].Value = line; // 記錄錯誤行
                        rowIndex++;
                    }
                }
            }

            // 根據最長的訊息長度設定Column寬度
            worksheet.Column(1).Width = maxFileNameLength;
            worksheet.Column(2).Width = maxErrorLength;

            package.SaveAs(new FileInfo(excelFilePath));
        }
    }
}