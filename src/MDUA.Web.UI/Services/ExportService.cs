using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using MDUA.Web.UI.Services.Interface;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
// ❌ REMOVED: using static System.Net.Mime.MediaTypeNames;

namespace MDUA.Web.UI.Services
{
    public class ExportService : IExportService
    {
        public ExportService()
        {
            // ✅ CORRECT CODE FOR EPPLUS 8
            ExcelPackage.License.SetNonCommercialPersonal("MDUA_Admin");
        }

        public byte[] GenerateFile(List<Dictionary<string, object>> data, string format, List<string> columns)
        {
            if (data == null || !data.Any())
            {
                return new byte[0];
            }

            switch (format.ToLower())
            {
                case "csv":
                    return GenerateCsv(data, columns);
                case "excel":
                    return GenerateExcel(data, columns);
                case "pdf":
                    return GeneratePdf(data, columns);
                default:
                    throw new NotImplementedException($"Format '{format}' is not supported.");
            }
        }

        private byte[] GenerateCsv(List<Dictionary<string, object>> data, List<string> columns)
        {
            var sb = new StringBuilder();

            // 1. Header
            sb.AppendLine(string.Join(",", columns));

            // 2. Data
            foreach (var row in data)
            {
                var line = new List<string>();
                foreach (var col in columns)
                {
                    var val = row.ContainsKey(col) && row[col] != null ? row[col].ToString() : "";
                    if (val.Contains(",") || val.Contains("\"") || val.Contains("\n"))
                    {
                        val = $"\"{val.Replace("\"", "\"\"")}\"";
                    }
                    line.Add(val);
                }
                sb.AppendLine(string.Join(",", line));
            }
            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        private byte[] GenerateExcel(List<Dictionary<string, object>> data, List<string> columns)
        {
            using (var package = new ExcelPackage())
            {
                var ws = package.Workbook.Worksheets.Add("ExportData");

                // 1. Header Row
                for (int i = 0; i < columns.Count; i++)
                {
                    ws.Cells[1, i + 1].Value = columns[i];
                    ws.Cells[1, i + 1].Style.Font.Bold = true;
                }

                // 2. Data Rows
                for (int r = 0; r < data.Count; r++)
                {
                    var row = data[r];
                    for (int c = 0; c < columns.Count; c++)
                    {
                        var colKey = columns[c];
                        var val = row.ContainsKey(colKey) ? row[colKey] : null;
                        ws.Cells[r + 2, c + 1].Value = val;
                    }
                }

                ws.Cells.AutoFitColumns();
                return package.GetAsByteArray();
            }
        }

        private byte[] GeneratePdf(List<Dictionary<string, object>> data, List<string> columns)
        {
            using (var stream = new MemoryStream())
            {
                var writer = new PdfWriter(stream);
                var pdf = new PdfDocument(writer);
                var document = new iText.Layout.Document(pdf);

                // Create fonts
                PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                PdfFont normalFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

                // Title
                document.Add(new Paragraph(new Text("Exported Data").SetFont(boldFont).SetFontSize(18))
                    .SetTextAlignment(TextAlignment.CENTER));

                // Table
                var table = new Table(UnitValue.CreatePercentArray(columns.Count)).UseAllAvailableWidth();

                // 1. Header
                foreach (var col in columns)
                {
                    table.AddHeaderCell(new Cell().Add(new Paragraph(new Text(col).SetFont(boldFont))));
                }

                // 2. Data
                foreach (var row in data)
                {
                    foreach (var col in columns)
                    {
                        var val = row.ContainsKey(col) && row[col] != null ? row[col].ToString() : "";
                        table.AddCell(new Paragraph(new Text(val).SetFont(normalFont)).SetFontSize(10));
                    }
                }

                document.Add(table);
                document.Close();
                return stream.ToArray();
            }
        }
    }
}