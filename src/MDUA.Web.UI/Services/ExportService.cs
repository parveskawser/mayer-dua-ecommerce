using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using MDUA.Web.UI.Services.Interface;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PdfBorder = iText.Layout.Borders.Border;
using PdfSolidBorder = iText.Layout.Borders.SolidBorder;

namespace MDUA.Web.UI.Services
{
    public class ExportService : IExportService
    {
        public ExportService()
        {
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
                case "xlsx":
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

                // Style the header row
                for (int i = 0; i < columns.Count; i++)
                {
                    var cell = ws.Cells[1, i + 1];
                    cell.Value = columns[i];
                    cell.Style.Font.Bold = true;
                    cell.Style.Font.Size = 12;
                    cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));
                    cell.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }

                // Add data rows with styling
                for (int r = 0; r < data.Count; r++)
                {
                    var row = data[r];
                    for (int c = 0; c < columns.Count; c++)
                    {
                        var colKey = columns[c];
                        var val = row.ContainsKey(colKey) ? row[colKey] : null;
                        var cell = ws.Cells[r + 2, c + 1];

                        cell.Value = val;

                        // Alternate row coloring
                        if (r % 2 == 0)
                        {
                            cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(242, 242, 242));
                        }

                        // Add borders
                        cell.Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.LightGray);

                        // Format numbers and dates
                        if (val is decimal || val is double || val is int)
                        {
                            cell.Style.Numberformat.Format = "#,##0.00";
                            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        }
                        else if (val is DateTime)
                        {
                            cell.Style.Numberformat.Format = "mmm dd, yyyy";
                        }
                    }
                }

                // Auto-fit columns
                ws.Cells.AutoFitColumns(0);

                // Freeze header row
                ws.View.FreezePanes(2, 1);

                return package.GetAsByteArray();
            }
        }

        private byte[] GeneratePdf(List<Dictionary<string, object>> data, List<string> columns)
        {
            using (var stream = new MemoryStream())
            {
                var writer = new PdfWriter(stream);
                var pdf = new PdfDocument(writer);

                // Set page size to landscape for better table fitting
                pdf.SetDefaultPageSize(PageSize.A4.Rotate());

                var document = new Document(pdf);
                document.SetMargins(30, 30, 30, 30);

                // Create fonts
                PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                PdfFont normalFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
                PdfFont italicFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_OBLIQUE);

                // ===== HEADER SECTION =====
                // Company/Report Title
                var titlePara = new Paragraph("ORDER EXPORT REPORT")
                    .SetFont(boldFont)
                    .SetFontSize(20)
                    .SetFontColor(ColorConstants.BLACK)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMarginBottom(5);
                document.Add(titlePara);

                // Subtitle with generation date
                var subtitlePara = new Paragraph($"Generated on {DateTime.Now:MMMM dd, yyyy HH:mm} (UTC {DateTime.UtcNow:MMMM dd, yyyy HH:mm})")
                     .SetFont(italicFont)
                    .SetFontSize(10)
                    .SetFontColor(new DeviceRgb(100, 100, 100))
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMarginBottom(10);
                document.Add(subtitlePara);

                // Separator line
                var line = new LineSeparator(new SolidLine(1f));
                line.SetMarginTop(5);
                line.SetMarginBottom(15);
                document.Add(line);

                // ===== SUMMARY SECTION =====
                var summaryTable = new Table(UnitValue.CreatePercentArray(new float[] { 30, 70 }))
                    .SetWidth(UnitValue.CreatePercentValue(40))
                    .SetHorizontalAlignment(HorizontalAlignment.LEFT)
                    .SetMarginBottom(15);

                AddSummaryRow(summaryTable, "Total Records:", data.Count.ToString(), normalFont, boldFont);
                AddSummaryRow(summaryTable, "Report Type:", "Sales Orders", normalFont, boldFont);

                document.Add(summaryTable);

                // ===== DATA TABLE =====
                // Calculate column widths dynamically
                float[] columnWidths = new float[columns.Count];
                for (int i = 0; i < columns.Count; i++)
                {
                    columnWidths[i] = 1; // Equal width by default
                }

                var table = new Table(UnitValue.CreatePercentArray(columnWidths))
                    .UseAllAvailableWidth()
                    .SetMarginTop(10);

                // Header row with styling
                foreach (var col in columns)
                {
                    var headerCell = new Cell()
                        .Add(new Paragraph(col)
                            .SetFont(boldFont)
                            .SetFontSize(9)
                            .SetFontColor(ColorConstants.WHITE))
                        .SetBackgroundColor(new DeviceRgb(79, 129, 189))
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                        .SetPadding(8)
                        .SetBorder(new PdfSolidBorder(ColorConstants.WHITE, 0.5f));

                    table.AddHeaderCell(headerCell);
                }

                // Data rows with alternating colors
                bool isAlternate = false;
                foreach (var row in data)
                {
                    foreach (var col in columns)
                    {
                        var val = row.ContainsKey(col) && row[col] != null ? row[col].ToString() : "-";

                        var dataCell = new Cell()
                            .Add(new Paragraph(val)
                                .SetFont(normalFont)
                                .SetFontSize(8))
                            .SetPadding(6)
                            .SetBorder(new PdfSolidBorder(new DeviceRgb(220, 220, 220), 0.5f));

                        // Alternating row colors
                        if (isAlternate)
                        {
                            dataCell.SetBackgroundColor(new DeviceRgb(242, 242, 242));
                        }
                        else
                        {
                            dataCell.SetBackgroundColor(ColorConstants.WHITE);
                        }

                        // Right-align numeric values
                        if (val.StartsWith("Tk") || decimal.TryParse(val, out _))
                        {
                            dataCell.SetTextAlignment(TextAlignment.RIGHT);
                        }

                        table.AddCell(dataCell);
                    }
                    isAlternate = !isAlternate;
                }

                document.Add(table);

                // ===== FOOTER SECTION =====
                document.Add(new Paragraph("\n"));
                var footerLine = new LineSeparator(new SolidLine(0.5f));
                footerLine.SetMarginTop(10);
                footerLine.SetMarginBottom(5);
                document.Add(footerLine);

                var footerPara = new Paragraph($"Page {pdf.GetNumberOfPages()} | Total Records: {data.Count} | © {DateTime.Now.Year} MDUA Admin")
                    .SetFont(italicFont)
                    .SetFontSize(8)
                    .SetFontColor(new DeviceRgb(128, 128, 128))
                    .SetTextAlignment(TextAlignment.CENTER);
                document.Add(footerPara);

                document.Close();
                return stream.ToArray();
            }
        }

        private void AddSummaryRow(Table table, string label, string value, PdfFont labelFont, PdfFont valueFont)
        {
            table.AddCell(new Cell()
                .Add(new Paragraph(label).SetFont(labelFont).SetFontSize(9))
                .SetBorder(PdfBorder.NO_BORDER)
                .SetPadding(2));

            table.AddCell(new Cell()
                .Add(new Paragraph(value).SetFont(valueFont).SetFontSize(9))
                .SetBorder(PdfBorder.NO_BORDER)
                .SetPadding(2));
        }
    }
}