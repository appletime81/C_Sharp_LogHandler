using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using Newtonsoft.Json;

namespace C_Sharp_LogHandler
{
    internal class EmailConfig
    {
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public List<string> EmailList { get; set; }
    }

    internal static class EmailSender
    {
        public static void SendEmailWithAttachment(string jsonConfigPath, string subject, string body,
            string attachmentFilePath)
        {
            var emailConfig = LoadEmailConfig(jsonConfigPath);

            using var client = new SmtpClient(emailConfig.SmtpServer, emailConfig.Port)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(emailConfig.Email, emailConfig.Password)
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(emailConfig.Email),
                Subject = subject,
                Body = body
            };

            foreach (var email in emailConfig.EmailList)
            {
                mailMessage.To.Add(email);
            }

            if (File.Exists(attachmentFilePath))
            {
                var attachment = new Attachment(attachmentFilePath);
                mailMessage.Attachments.Add(attachment);
            }
            else
            {
                Console.WriteLine($"Warning: Attachment file not found at {attachmentFilePath}");
            }

            client.Send(mailMessage);
        }

        private static EmailConfig LoadEmailConfig(string path)
        {
            var json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<EmailConfig>(json);
        }
    }
}