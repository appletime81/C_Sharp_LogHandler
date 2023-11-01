namespace C_Sharp_LogHandler;

using System.IO;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;

public class StatisticsCount
{
    // read excel file
    public static Dictionary<string, int> ReadExcel(string relativeExcelPath)
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var excelFilePath = Path.Combine(currentDirectory, relativeExcelPath);

        using var package = new ExcelPackage(new FileInfo(excelFilePath));
        var worksheet = package.Workbook.Worksheets[0];

        // get row count
        int rowCount = worksheet.Dimension.Rows;

        // get column count
        int colCount = worksheet.Dimension.Columns;

        // get cell value & count error type
        var errorStats = new Dictionary<string, int>();
        for (int row = 1; row <= rowCount; row++)
        {
            for (int col = 1; col <= colCount; col++)
            {
                // if col is 2
                if (col == 2)
                {
                    var cellValue = worksheet.Cells[row, col].Value.ToString();
                    if (!string.IsNullOrEmpty(cellValue))
                    {
                        var matches = Regex.Matches(cellValue, @"\[(.*?)\] ERROR - \[(.*?)\]");
                        foreach (Match match in matches)
                        {
                            var errorType = match.Groups[2].Value;
                            if (errorStats.ContainsKey(errorType))
                            {
                                errorStats[errorType]++;
                            }
                            else
                            {
                                errorStats.Add(errorType, 1);
                            }
                        }
                    }
                }
            }
        }

        return errorStats;
    }

    // generate html stats report
    public static string GenerateHTMLFromStats(Dictionary<string, int> errorStats)
    {
        var totalErrors = 0;
        foreach (var error in errorStats)
        {
            totalErrors += error.Value;
        }

        var colors = new string[]
        {
            "#FF5733", "#33FF57", "#3357FF", "#FF33A1", "#FFFF33", // Bright colors
            "#FF8C00", "#20B2AA", "#9400D3", "#A52A2A", "#8A2BE2", // Various shades
            "#5F9EA0", "#7FFF00", "#D2691E", "#FF7F50", "#6495ED", // Pastels & others
            "#FFF8DC", "#DC143C", "#00FFFF", "#00008B", "#008B8B", // Mixed set
            "#B8860B", "#A9A9A9", "#006400", "#BDB76B", "#8B008B", // Dark shades
            "#556B2F", "#FF8C00", "#9932CC", "#8B0000", "#E9967A", // Earthy & rich
            "#8FBC8F", "#483D8B", "#2F4F4F", "#00CED1", "#9400D3", // Muted shades
            "#FF1493", "#00BFFF", "#696969", "#1E90FF", "#B22222"
        }; // Sample colors, you can add more if needed
        var sb = new StringBuilder();

        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html lang=\"en\">");
        // ... rest of the head content ...

        sb.AppendLine(
            "<body style=\"font-family: 'Arial', sans-serif; background-color: #edf2f7; padding: 20px; color: #2d3748;\">");
        sb.AppendLine(
            "<h2 style=\"text-align: center; margin-bottom: 20px; font-size: 24px; font-weight: 700;\">Error Events Statistics</h2>");
        sb.AppendLine(
            "<div id=\"statistics\" style=\"max-width: 600px; margin: 0 auto; padding: 20px; background-color: #fff; box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06); border-radius: 0.25rem;\">");

        int colorIndex = 0;
        foreach (var error in errorStats)
        {
            var percentage = ((double)error.Value / totalErrors) * 100;
            sb.AppendLine("<div style=\"margin-bottom: 10px;\">");
            sb.AppendLine($"<div style=\"font-weight: bold; margin-bottom: 5px;\">{error.Key}</div>");
            sb.AppendLine(
                $"<div style=\"background-color: {colors[colorIndex % colors.Length]}; height: 20px; width: {percentage}%;\"></div>");
            sb.AppendLine(
                $"<div style=\"margin-top: 5px;\">{percentage.ToString("0.##")}% ({error.Value} times) <br><br></div>");
            sb.AppendLine("</div>");

            colorIndex++;
        }

        sb.AppendLine("</div>");
        sb.AppendLine("</body>");
        sb.AppendLine("</html>");

        return sb.ToString();
    }
}