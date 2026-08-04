
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ScrumBoard.Application.Ports;
using ScrumBoard.Application.Reports;
namespace ScrumBoard.Infrastructure.Reports;

public class PdfReportExporter : IReportExporter
{
    public string Format => "PDF";
    public string ContentType => "application/pdf";
    public string FileExtension => ".pdf";

    public byte[] Export(ProjectReportDto data)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Element(compose => ComposeHeader(compose, data));
                page.Content().Element(compose => ComposeContent(compose, data));
                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Página ");
                    x.CurrentPageNumber();
                    x.Span(" de ");
                    x.TotalPages();
                });
            });
        });

        return document.GeneratePdf();
    }

    private void ComposeHeader(IContainer container, ProjectReportDto data)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item().Text($"Reporte de Proyecto: {data.ProjectName}").FontSize(20).SemiBold().FontColor(Colors.Blue.Darken2);
                column.Item().Text($"Generado el: {data.GeneratedAt:dd/MM/yyyy HH:mm}");
            });
        });
    }

    private void ComposeContent(IContainer container, ProjectReportDto data)
    {
        container.PaddingVertical(1, Unit.Centimetre).Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(3); // Tarea
                columns.RelativeColumn(2); // Columna
                columns.RelativeColumn(2); // Responsable
                columns.RelativeColumn(2); // Prioridad
            });

            table.Header(header =>
            {
                header.Cell().Element(CellStyle).Text("Tarea");
                header.Cell().Element(CellStyle).Text("Columna");
                header.Cell().Element(CellStyle).Text("Responsable");
                header.Cell().Element(CellStyle).Text("Prioridad");

                static IContainer CellStyle(IContainer container)
                {
                    return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                }
            });

            foreach (var task in data.Tasks)
            {
                table.Cell().Element(CellStyle).Text(task.TaskTitle);
                table.Cell().Element(CellStyle).Text(task.ColumnName);
                table.Cell().Element(CellStyle).Text(task.ResponsibleName);
                table.Cell().Element(CellStyle).Text(task.Priority);

                static IContainer CellStyle(IContainer container)
                {
                    return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                }
            }
        });
    }
}