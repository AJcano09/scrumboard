using ScrumBoard.Application.Reports;

namespace ScrumBoard.Application.Ports;

public interface IReportExporter
{
    string Format { get; } // ej. "PDF" o "EXCEL"
    string ContentType { get; }
    string FileExtension { get; }
    byte[] Export(ProjectReportDto data);
}