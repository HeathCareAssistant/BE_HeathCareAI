using FluentEmail.Core;
using System;
using System.Threading.Tasks;
using System.Net.Mail;
using HealthyCareAssistant.Contract.Service.Interface;
using HealthyCareAssistant.ModelViews.UserModelViews;

public class EmailService : IEmailService
{
    private readonly IFluentEmail _fluentEmail;

    public EmailService(IFluentEmail fluentEmail)
    {
        _fluentEmail = fluentEmail ?? throw new ArgumentNullException(nameof(fluentEmail));
    }

    public async Task<bool> SendEmailAsync(EmailMetadata emailMetadata)
    {
        try
        {
            var response = await _fluentEmail
                .To(emailMetadata.ToAddress)
                .Subject(emailMetadata.Subject)
                .Body(emailMetadata.Body, isHtml: true)
                .SendAsync();

            if (response.Successful)
            {
                Console.WriteLine($"✅ [EmailService] Email sent to {emailMetadata.ToAddress}");
                return true;
            }
            else
            {
                Console.WriteLine($"❌ [EmailService] Email sending failed: {string.Join(", ", response.ErrorMessages)}");
                return false;
            }
        }
        catch (SmtpException smtpEx)
        {
            Console.WriteLine($"❌ [EmailService] SMTP Exception: {smtpEx.StatusCode} - {smtpEx.Message}");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ [EmailService] GeneralFailure - {ex.Message}");
            return false;
        }
    }
}
