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
        };
        var sb = new StringBuilder();

        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html lang=\"en\">");
        sb.AppendLine("<head>");
        sb.AppendLine("    <meta charset=\"UTF-8\">");
        sb.AppendLine("    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
        sb.AppendLine("    <title>Error Events Statistics</title>");
        sb.AppendLine("</head>");
        sb.AppendLine(
            "<body style=\"font-family: 'Verdana', sans-serif; background-color: #edf2f7; color: #2d3748; text-align: center;\">");
        sb.AppendLine(
            "<h2 style=\"margin-bottom: 30px; font-size: 26px; font-weight: bold;\">Error Events Statistics</h2>");
        sb.AppendLine(
            "<table id=\"statistics\" align=\"center\" style=\"width:600px; margin:0 auto; padding:20px; background-color:#fff; border-collapse: collapse;\" cellspacing=\"0\" cellpadding=\"0\">");

        int colorIndex = 0;
        foreach (var error in errorStats)
        {
            var percentage = ((double)error.Value / totalErrors) * 100;

            sb.AppendLine("<tr style=\"font-size: 16px;\">");
            sb.AppendLine(
                $"<td style=\"font-weight: bold; padding-bottom: 20px; padding-top: 20px; width: 50%; text-align: left;\">{error.Key}</td>");
            sb.AppendLine(
                $"<td style=\"width: 30%; padding: 0;\"><table style=\"border-collapse: collapse; width: 100%;\"><tr><td style=\"background-color: {colors[colorIndex % colors.Length]}; height: 20px; width:{percentage.ToString("0.##")}%;\"></td><td style=\"background-color: #edf2f7;\"></td></tr></table></td>");
            sb.AppendLine(
                $"<td style=\"font-size: 14px; padding-left: 20px; width: 20%; text-align: right;\">{percentage.ToString("0.##")}%<br><span style=\"font-size: 12px;\">({error.Value} events)</span></td>");
            sb.AppendLine("</tr>");

            colorIndex++;
        }

        sb.AppendLine("</table>");
        sb.AppendLine("</body>");
        sb.AppendLine("</html>");
        // Console.WriteLine(sb.ToString());
        return sb.ToString();
    }
}