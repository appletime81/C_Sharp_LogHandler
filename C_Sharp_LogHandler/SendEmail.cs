using System;
using System.Text.Json;
using System.IO;
using MailKit.Net.Smtp;
using MimeKit;

public static class EmailSender
{
    public static void SendEmailWithAttachment(string jsonConfigPath, string subject, string body,
        string attachmentFilePath)
    {
        var jsonString = File.ReadAllText(jsonConfigPath);
        var config = JsonSerializer.Deserialize<EmailConfig>(jsonString);

        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(config.SenderEmail));
        foreach (var email in config.EmailList)
        {
            message.To.Add(MailboxAddress.Parse(email));
        }

        message.Subject = subject;
        message.Body = new TextPart("html") { Text = body };

        if (!string.IsNullOrEmpty(attachmentFilePath) && File.Exists(attachmentFilePath))
        {
            var attachment = new MimePart("application", "octet-stream")
            {
                Content = new MimeContent(File.OpenRead(attachmentFilePath)),
                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = Path.GetFileName(attachmentFilePath)
            };
            var multipart = new Multipart("mixed");
            multipart.Add(message.Body);
            multipart.Add(attachment);
            message.Body = multipart;
        }

        using var client = new SmtpClient();
        try
        {
            client.Connect(config.SmtpServer, config.Port, true); // The 'true' specifies SSL
            client.Authenticate(config.Email, config.Password);
            client.Send(message);
            client.Disconnect(true);
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