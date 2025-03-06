using FluentEmail.Core;
using FluentEmail.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class FluentEmailExtensions
{
    public static void AddFluentEmailService(this IServiceCollection services, ConfigurationManager configuration)
    {
        var emailSettings = configuration.GetSection("EmailSettings");
        var defaultFromEmail = emailSettings["DefaultFromEmail"];
        var smtpConfig = emailSettings.GetSection("SMTPSetting");

        var host = smtpConfig["Host"];
        var port = smtpConfig.GetValue<int>("Port");
        var userName = smtpConfig["UserName"];
        var password = smtpConfig["Password"];

        services
            .AddFluentEmail(defaultFromEmail)
            .AddRazorRenderer()
            .AddSmtpSender(host, port, userName, password);
    }
}
