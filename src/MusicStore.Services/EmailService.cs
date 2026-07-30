using System.Net.Mail;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MusicStore.Entities;

namespace MusicStore.Services;

public class EmailService(
    IOptions<AppSettings> options,
    ILogger<EmailService> logger
) : IEmailService
{
    public async Task SendEmailAsync(string email, string subject, string message)
    {
        var smtp = options.Value.Smtp;
        var mailMessage = new MailMessage(new MailAddress(smtp.UserName, smtp.FromName),
            new MailAddress(email));

        mailMessage.Subject = subject;
        mailMessage.Body = message;
        mailMessage.IsBodyHtml = true;

        using var smtpClient = new SmtpClient(smtp.Server, smtp.Port)
        {
            Credentials = new System.Net.NetworkCredential(smtp.UserName, smtp.Password),
            EnableSsl = smtp.EnableSsl,
            UseDefaultCredentials = false
        };

        try
        {
            await smtpClient.SendMailAsync(mailMessage);
            logger.LogInformation("Email sent to {Email}", email);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send email to {Email}: {Message}", email, ex.Message);
            throw;
        }
    }
}