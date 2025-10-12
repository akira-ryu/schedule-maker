using subcinctus_factorem.Employees;

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
            try
            {
                if (string.IsNullOrWhiteSpace(employee.Email))
                {
                    return false;
                }

                // Generate PDF if it doesn't exist or wasn't provided
                if (string.IsNullOrWhiteSpace(pdfFilePath) || !File.Exists(pdfFilePath))
                {
                    pdfFilePath = _pdfService.GenerateSchedulePdf(employee);
                }

                var message = new EmailMessage
                {
                    Subject = $"Your Schedule - Week of {DateTime.Now:MMM dd, yyyy}",
                    Body = $@"Hi {employee.Name},

Please find attached your schedule for the current week.

Schedule Details:
{GetScheduleSummary(employee)}

If you have any questions or concerns about your schedule, please contact your manager.

Best regards,
Management Team",
                    To = new List<string> { employee.Email }
                };

                // Attach PDF
                if (File.Exists(pdfFilePath))
                {
                    message.Attachments.Add(new EmailAttachment(pdfFilePath));
                }

                await Email.Default.ComposeAsync(message);
                return true;
            }
            catch (Exception)
            {
                return false;
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