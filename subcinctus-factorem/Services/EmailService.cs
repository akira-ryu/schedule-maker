using subcinctus_factorem.Employees;
using System.Diagnostics;

namespace subcinctus_factorem.Services
{
    public class EmailService
    {
        private readonly PdfService _pdfService;

        public EmailService()
        {
            _pdfService = new PdfService();
        }

        public async Task<bool> SendScheduleEmail(Employee employee, string pdfFilePath = null)
        {
            if (string.IsNullOrWhiteSpace(employee.Email))
            {
                throw new Exception("Employee email is empty");
            }

            // Generate PDF if it doesn't exist or wasn't provided
            if (string.IsNullOrWhiteSpace(pdfFilePath) || !File.Exists(pdfFilePath))
            {
                pdfFilePath = _pdfService.GenerateSchedulePdf(employee);
            }

            if (!File.Exists(pdfFilePath))
            {
                throw new Exception($"PDF file not found at: {pdfFilePath}");
            }

            // Open email client with mailto and show PDF location
            await Task.Run(() => OpenEmailWithPdf(employee, pdfFilePath));
            return true;
        }

        private void OpenEmailWithPdf(Employee employee, string pdfFilePath)
        {
            try
            {
                // Create email body with instructions
                string subject = $"Your Schedule - Week of {DateTime.Now:MMM dd, yyyy}";
                string body = $@"Hi {employee.Name},

Please find attached your schedule for the current week.

Schedule Details:
{GetScheduleSummary(employee)}

If you have any questions or concerns about your schedule, please contact your manager.

Best regards,
Management Team";

                // Escape for mailto URL
                string mailtoSubject = Uri.EscapeDataString(subject);
                string mailtoBody = Uri.EscapeDataString(body);

                // Build mailto URL
                string mailtoUrl = $"mailto:{employee.Email}?subject={mailtoSubject}&body={mailtoBody}";
                // Open File Explorer with the PDF selected so user can drag-drop it
                Process.Start(new ProcessStartInfo
                {
                    FileName = "explorer.exe",
                    Arguments = $"/select,\"{Path.GetFullPath(pdfFilePath)}\"",
                    UseShellExecute = true
                });
                System.Threading.Thread.Sleep(1000);
                // Open default email client with the mailto URL
                Process.Start(new ProcessStartInfo
                {
                    FileName = mailtoUrl,
                    UseShellExecute = true
                });

                // Wait a moment for email client to open
                

                
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to open email client: {ex.Message}");
            }
        }

        private string GetScheduleSummary(Employee employee)
        {
            var summary = "";
            foreach (var schedule in employee.Schedule)
            {
                if (!string.IsNullOrWhiteSpace(schedule.Position) &&
                    !string.IsNullOrWhiteSpace(schedule.Start) &&
                    !string.IsNullOrWhiteSpace(schedule.End))
                {
                    summary += $"• {schedule.Date:ddd, MMM dd}: {schedule.Position} ({schedule.Start} - {schedule.End})\n";
                }
            }
            return string.IsNullOrWhiteSpace(summary) ? "No shifts scheduled" : summary;
        }
    }
}