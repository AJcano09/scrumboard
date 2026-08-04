using ClosedXML.Excel;
using ScrumBoard.Application.Ports;

namespace ScrumBoard.Application.Reports;

public class ExcelReportExporter : IReportExporter
{
    public string Format => "EXCEL";
    public string ContentType => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    public string FileExtension => ".xlsx";

    public byte[] Export(ProjectReportDto data)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Reporte de Tareas");

        // Encabezado del proyecto
        worksheet.Cell(1, 1).Value = $"Reporte de Proyecto: {data.ProjectName}";
        worksheet.Cell(1, 1).Style.Font.Bold = true;
        worksheet.Cell(1, 1).Style.Font.FontSize = 14;
        
        worksheet.Cell(2, 1).Value = $"Generado el: {data.GeneratedAt:dd/MM/yyyy HH:mm}";

        // Encabezados de tabla
        var headerRow = 4;
        worksheet.Cell(headerRow, 1).Value = "Tarea";
        worksheet.Cell(headerRow, 2).Value = "Columna";
        worksheet.Cell(headerRow, 3).Value = "Responsable";
        worksheet.Cell(headerRow, 4).Value = "Prioridad";

        var headerRange = worksheet.Range(headerRow, 1, headerRow, 4);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

        // Datos
        var currentRow = 5;
        foreach (var task in data.Tasks)
        {
            worksheet.Cell(currentRow, 1).Value = task.TaskTitle;
            worksheet.Cell(currentRow, 2).Value = task.ColumnName;
            worksheet.Cell(currentRow, 3).Value = task.ResponsibleName;
            worksheet.Cell(currentRow, 4).Value = task.Priority;
            currentRow++;
        }

        // Ajustar anchos de columna
        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}