using Microsoft.AspNetCore.Mvc;
using ScrumBoard.Application.Ports;

namespace ScrumBoard.Api.Controllers;

[ApiController]
[Route("api/projects/{projectId:guid}/reports")]
public class ReportsController(
    IProjectReportQuery reportQuery,
    IEnumerable<IReportExporter> exporters) : ControllerBase
{
    [HttpGet("{format}")]
    public async Task<IActionResult> DownloadReport(Guid projectId, string format)
    {
        var exporter = exporters.FirstOrDefault(e => e.Format.Equals(format, StringComparison.OrdinalIgnoreCase));
        if (exporter == null)
            return BadRequest(new { message = "Formato no soportado." });

        var data = await reportQuery.GetProjectReportDataAsync(projectId);
        if (data == null)
            return NotFound(new { message = "Proyecto no encontrado." });

        var fileBytes = exporter.Export(data);
        var fileName = $"Reporte_{data.ProjectName.Replace(" ", "_")}_{DateTime.UtcNow:yyyyMMdd}{exporter.FileExtension}";

        return File(fileBytes, exporter.ContentType, fileName);
    }
}