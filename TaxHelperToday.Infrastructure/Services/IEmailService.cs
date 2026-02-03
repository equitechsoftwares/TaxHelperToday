namespace TaxHelperToday.Infrastructure.Services;

public interface IEmailService
{
    Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = true, string? fromName = null);
    Task<bool> SendEnquiryNotificationAsync(string enquiryType, string name, string email, string? phone, string subject, string message, string? serviceName = null);
    Task<bool> SendEnquiryConfirmationAsync(string to, string name, string enquiryType);
}
