using System;
using System.Net;
using System.Net.Mail;
using System.Text.Json;
using System.IO;

public static class EmailSender
{
    public static void SendEmailWithAttachment(string jsonConfigPath, string subject, string body,
        string attachmentFilePath)
    {
        var jsonString = File.ReadAllText(jsonConfigPath);
        var config = JsonSerializer.Deserialize<EmailConfig>(jsonString);

        using var message = new MailMessage();
        message.From = new MailAddress(config.SenderEmail);
        foreach (var email in config.EmailList)
        {
            message.To.Add(email);
        }

        message.Subject = subject;
        message.Body = body;

        if (!string.IsNullOrEmpty(attachmentFilePath) && File.Exists(attachmentFilePath))
        {
            var attachment = new Attachment(attachmentFilePath);
            message.Attachments.Add(attachment);
        }

        using var client = new SmtpClient(config.SmtpServer, config.Port)
        {
            Credentials = new NetworkCredential(config.Email, config.Password),
            EnableSsl = true,
            UseDefaultCredentials = false
        };

        try
        {
            client.Send(message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error sending email: " + ex.Message);
        }
    }

    public class EmailConfig
    {
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string SenderEmail { get; set; }
        public string[] EmailList { get; set; }
    }
}