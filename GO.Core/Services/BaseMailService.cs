namespace GO.Core.Services
{
    using System;
    using System.IO;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using GO.Core.Criteria.MailRecord;
    using GO.Core.Models;
    using MailKit.Net.Smtp;
    using MimeKit;

    public class BaseMailService : IBaseMailService
    {
        private readonly MailConfigs mailConfigs;
        private readonly IBaseService baseService;

        public BaseMailService(
            MailConfigs mailConfigs,
            IBaseService baseService)
        {
            this.mailConfigs = mailConfigs;
            this.baseService = baseService;
        }

        public async Task SendMailAsync(MailModel email)
        {
            if (email == null || !IsValidEmail(email.MailTo) || !mailConfigs.Enabled)
            {
                return;
            }

            using var mailClient = new SmtpClient();
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(mailConfigs.SenderName, mailConfigs.Username));
                message.To.Add(new MailboxAddress(string.Empty, email.MailTo));
                message.Subject = email.Subject;
                message.Priority = mailConfigs.MailPriority;

                var builder = new BodyBuilder
                {
                    HtmlBody = email.Body
                };
                message.Body = builder.ToMessageBody();

                mailClient.ServerCertificateValidationCallback = RemoteCertificateValidationCallback;
                await mailClient.ConnectAsync(mailConfigs.SmtpServer, mailConfigs.SmtpPort, mailConfigs.EnableSsl);
                if (mailConfigs.EnableSsl)
                {
                    await mailClient.AuthenticateAsync(mailConfigs.Username, mailConfigs.Password);
                }

                await mailClient.SendAsync(message);
                await InsertMailAsync(email);
            }
            catch (Exception ex)
            {
                await InsertMailErrorAsync(email, ex);
            }

            await mailClient.DisconnectAsync(true);
        }

        public string ReadTextFile(string filePath)
        {
            using var sr = new StreamReader(filePath);
            return sr.ReadToEnd();
        }

        private static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            var regex = new Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
            return regex.IsMatch(email);
        }

        private async Task<CommandResult> InsertMailAsync(MailModel email)
        {
            var command = new InsertMailRecordCommand
            {
                MailTo = email.MailTo,
                Subject = email.Subject,
                Body = email.Body
            };
            return await baseService.ExecuteNonQueryAsync(command);
        }

        private async Task<CommandResult> InsertMailErrorAsync(MailModel email, Exception ex)
        {
            var command = new InsertMailErrorCommand
            {
                MailTo = email.MailTo,
                Subject = email.Subject,
                Body = email.Body,
                Message = ex.Message,
                StackTrace = ex.StackTrace
            };
            return await baseService.ExecuteNonQueryAsync(command);
        }

        private static bool RemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) => true;
    }
}