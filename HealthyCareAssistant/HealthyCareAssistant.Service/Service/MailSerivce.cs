using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using HealthyCareAssistant.Service.Config;
using HealthyCareAssistant.Contract.Service.Interface;
using HealthyCareAssistant.ModelViews.MailModelViews;

public class MailService : IMailService
{
    private readonly MailSettings _mailSettings;

    public MailService(IOptions<MailSettings> mailSettingsOptions)
    {
        _mailSettings = mailSettingsOptions.Value;

        Console.WriteLine($"[MailService] Loaded Mail Settings:");
        Console.WriteLine($"- Server: {_mailSettings.Server}");
        Console.WriteLine($"- Port: {_mailSettings.Port}");
        Console.WriteLine($"- SenderEmail: {_mailSettings.SenderEmail}");
        Console.WriteLine($"- UserName: {_mailSettings.UserName}");

        if (string.IsNullOrWhiteSpace(_mailSettings.SenderEmail))
        {
            Console.WriteLine("[MailService] Error: SenderEmail is null.");
            throw new ArgumentNullException(nameof(_mailSettings.SenderEmail), "SenderEmail cannot be null in appsettings.json");
        }
    }

    public async Task<bool> SendEmailAsync(EmailData mailData)
    {
        try
        {
            // 🔹 Kiểm tra dữ liệu email trước khi gửi
            if (string.IsNullOrWhiteSpace(mailData.EmailToId))
            {
                Console.WriteLine("[MailService] Error: EmailToId is null or empty.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(mailData.EmailToName))
            {
                mailData.EmailToName = "User";
            }

            if (string.IsNullOrWhiteSpace(mailData.EmailSubject))
            {
                mailData.EmailSubject = "No Subject";
            }

            if (string.IsNullOrWhiteSpace(mailData.EmailBody))
            {
                mailData.EmailBody = "No Content";
            }

            // Kiểm tra SenderEmail có null không
            if (string.IsNullOrWhiteSpace(_mailSettings.SenderEmail))
            {
                Console.WriteLine("[MailService] Error: SenderEmail is null.");
                return false;
            }

            using var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_mailSettings.SenderName ?? "HealthyCare", _mailSettings.SenderEmail));
            emailMessage.To.Add(new MailboxAddress(mailData.EmailToName, mailData.EmailToId));
            emailMessage.Subject = mailData.EmailSubject;
            emailMessage.Body = new TextPart("html") { Text = mailData.EmailBody };

            Console.WriteLine($"[MailService] Sending email to: {mailData.EmailToId} with sender: {_mailSettings.SenderEmail}");

            using var client = new SmtpClient();
            await client.ConnectAsync(_mailSettings.Server, _mailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_mailSettings.UserName, _mailSettings.Password);
            await client.SendAsync(emailMessage);
            await client.DisconnectAsync(true);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[MailService] Error sending email: {ex.Message}");
            return false;
        }
    }

}
