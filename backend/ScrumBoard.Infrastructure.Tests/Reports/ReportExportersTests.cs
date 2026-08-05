using System.Text;
using ScrumBoard.Application.Ports;
using ScrumBoard.Application.Reports;
using ScrumBoard.Infrastructure.Reports;

namespace ScrumBoard.Infrastructure.Tests.Reports;

/// <summary>
/// Valida el patrón Strategy de exportación dual (PDF + Excel):
/// ambos exportadores exponen la misma interfaz IReportExporter,
/// consumen el mismo DTO, y producen salidas distintas.
/// Demostración de extensibilidad: agregar un tercer formato
/// no requiere tocar controladores ni queries existentes.
/// </summary>
public class ReportExportersTests
{
    private static ProjectReportDto GetSampleData() => new(
        Guid.NewGuid(),
        "Proyecto Test",
        DateTime.UtcNow,
        new List<TaskReportItemDto>
        {
            new("Tarea 1", "To Do", "Ana", "Alta"),
            new("Tarea 2", "Haciendo", "Luis", "Media"),
            new("Tarea 3", "Bloq", "Ana", "Baja")
        });

    [Fact]
    public void PdfReportExporter_Format_ContentType_Y_FileExtension_Correctos()
    {
        var exporter = new PdfReportExporter();

        Assert.Equal("PDF", exporter.Format);
        Assert.Equal("application/pdf", exporter.ContentType);
        Assert.Equal(".pdf", exporter.FileExtension);
    }

    [Fact]
    public void ExcelReportExporter_Format_ContentType_Y_FileExtension_Correctos()
    {
        var exporter = new ExcelReportExporter();

        Assert.Equal("EXCEL", exporter.Format);
        Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", exporter.ContentType);
        Assert.Equal(".xlsx", exporter.FileExtension);
    }

    [Fact]
    public void PdfReportExporter_Export_ConDtoValido_DevuelvePdfNoVacio()
    {
        var exporter = new PdfReportExporter();
        var data = GetSampleData();

        var result = exporter.Export(data);

        Assert.NotEmpty(result);
        // PDF magic bytes: '%PDF'
        Assert.Equal("%PDF", Encoding.ASCII.GetString(result[0..4]));
    }

    [Fact]
    public void ExcelReportExporter_Export_ConDtoValido_DevuelveXlsxNoVacio()
    {
        var exporter = new ExcelReportExporter();
        var data = GetSampleData();

        var result = exporter.Export(data);

        Assert.NotEmpty(result);
        // XLSX is a ZIP archive — magic bytes: 'PK'
        Assert.Equal("PK", Encoding.ASCII.GetString(result[0..2]));
    }

    [Fact]
    public void PdfReportExporter_Export_ConListaVacia_DevuelvePdfValido()
    {
        var exporter = new PdfReportExporter();
        var data = new ProjectReportDto(Guid.NewGuid(), "Proyecto Vacío", DateTime.UtcNow, new List<TaskReportItemDto>());

        var result = exporter.Export(data);

        Assert.NotEmpty(result);
        Assert.Equal("%PDF", Encoding.ASCII.GetString(result[0..4]));
    }

    [Fact]
    public void AmbosExportadores_ImplementanIReportExporter()
    {
        // Evidencia del patrón Strategy: ambas implementaciones son intercambiables
        IReportExporter pdf = new PdfReportExporter();
        IReportExporter excel = new ExcelReportExporter();

        Assert.Equal("PDF", pdf.Format);
        Assert.Equal("EXCEL", excel.Format);
        Assert.NotEqual(pdf.Format, excel.Format);
    }
}
