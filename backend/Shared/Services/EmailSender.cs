using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Net;
using System.Net.Mail;

namespace Shared.Services;



public class EmailSender {
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<EmailSender> _logger;

    public EmailSender(IConfiguration configuration, IWebHostEnvironment environment, ILogger<EmailSender> logger) {
        _configuration = configuration;
        _environment = environment;
        _logger = logger;
    }

    public async Task SendAsync(string email, string title, string body, bool isHtml = true) {
        try {
            var emailFlowValue = _configuration.GetSection("EmailFlow").Value;
            if (!string.IsNullOrEmpty(emailFlowValue) && emailFlowValue.ToLower() == "true") {
                var host = _configuration.GetSection("EmailSenderSettings:Server").Value ??
                    throw new Exception("EmailSenderSettings:Server is not found in config");
                var addressFrom = _configuration.GetSection("EmailSenderSettings:Address").Value ??
                    throw new Exception("EmailSenderSettings:Address is not found in config");
                var password = _configuration.GetSection("EmailSenderSettings:Password").Value ??
                    throw new Exception("EmailSenderSettings:Password is not found in config");
                var port = _configuration.GetSection("EmailSenderSettings:Port").Value ??
                    throw new Exception("EmailSenderSettings:Port is not found in config");

                using (var client = new SmtpClient()) {
                    client.Host = host;
                    client.Port = int.Parse(port);
                    client.Credentials = new NetworkCredential(addressFrom, password);
                    client.Timeout = 100000;
                    client.EnableSsl = true;
                    client.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);

                    var mailMessage = new MailMessage(addressFrom, email, title, body) {
                        IsBodyHtml = isHtml
                    };
                    await client.SendMailAsync(mailMessage);
                }
            }

        } catch (Exception ex) {
            _logger.LogError(ex, "Exception occurs while email was sending");
            throw;
        }
    }

    private void SendCompletedCallback(object sender, AsyncCompletedEventArgs e) {
        if (e.Cancelled)
            _logger.LogWarning("Sending mail message was canceled");
        else if (e.Error != null)
            _logger.LogWarning("Sending mail message was with errors:{@Error}", e.Error);
        else
            _logger.LogInformation("Mail message was sended successful");
    }
}

