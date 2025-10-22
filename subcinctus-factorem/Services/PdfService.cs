using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using subcinctus_factorem.Employees;
using Colors = QuestPDF.Helpers.Colors;

namespace subcinctus_factorem.Services
{
    public class PdfService
    {
        public string GenerateSchedulePdf(Employee employee)
        {
            // Set QuestPDF license (Community license is free for open source)
            QuestPDF.Settings.License = LicenseType.Community;

            // Save to Documents folder instead of AppDataDirectory
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string schedulesFolder = Path.Combine(documentsPath, "EmployeeSchedules");

            // Create directory if it doesn't exist
            Directory.CreateDirectory(schedulesFolder);

            // Create file path
            string fileName = $"{employee.Name.Replace(" ", "_")}_Schedule_{DateTime.Now:yyyyMMdd}.pdf";
            string filePath = Path.Combine(schedulesFolder, fileName);

            // Generate PDF
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(50);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12).FontColor(Colors.Black));

                    page.Header()
                        .AlignCenter()
                        .Column(column =>
                        {
                            column.Item().PaddingBottom(10).Text("EMPLOYEE SCHEDULE")
                                .FontSize(24)
                                .Bold()
                                .FontColor("#57564F");

                            column.Item().PaddingBottom(5).Text(employee.Name)
                                .FontSize(18)
                                .SemiBold()
                                .FontColor("#57564F");

                            column.Item().Text(employee.Email)
                                .FontSize(12)
                                .FontColor("#7A7A73");

                            column.Item().PaddingTop(5).Text($"Generated: {DateTime.Now:MMMM dd, yyyy}")
                                .FontSize(10)
                                .FontColor("#7A7A73");
                        });

                    page.Content()
                        .PaddingTop(20)
                        .Column(column =>
                        {
                            // Table header
                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(2); // Date
                                    columns.RelativeColumn(2); // Position
                                    columns.RelativeColumn(1.5f); // Start
                                    columns.RelativeColumn(1.5f); // End
                                });

                                // Header row
                                table.Header(header =>
                                {
                                    header.Cell().Background("#57564F").Padding(10)
                                        .Text("DATE").FontColor(Colors.White).Bold();
                                    header.Cell().Background("#57564F").Padding(10)
                                        .Text("POSITION").FontColor(Colors.White).Bold();
                                    header.Cell().Background("#57564F").Padding(10)
                                        .Text("START").FontColor(Colors.White).Bold();
                                    header.Cell().Background("#57564F").Padding(10)
                                        .Text("END").FontColor(Colors.White).Bold();
                                });

                                // Data rows
                                foreach (var schedule in employee.Schedule)
                                {
                                    var bgColor = employee.Schedule.IndexOf(schedule) % 2 == 0
                                        ? "#F8F3CE" : "#DDDAD0";

                                    table.Cell().Background(bgColor).Padding(10)
                                        .Text(schedule.Date.ToString("ddd, MMM dd"))
                                        .FontColor("#57564F");

                                    table.Cell().Background(bgColor).Padding(10)
                                        .Text(schedule.Position ?? "-")
                                        .FontColor("#57564F");

                                    table.Cell().Background(bgColor).Padding(10)
                                        .Text(schedule.Start ?? "-")
                                        .FontColor("#57564F");

                                    table.Cell().Background(bgColor).Padding(10)
                                        .Text(schedule.End ?? "-")
                                        .FontColor("#57564F");
                                }
                            });
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.DefaultTextStyle(TextStyle.Default.FontSize(10).FontColor("#7A7A73"));
                            x.Span("Page ");
                            x.CurrentPageNumber();
                            x.Span(" of ");
                            x.TotalPages();
                        });
                });
            })
            .GeneratePdf(filePath);

            return filePath;
        }
    }
}