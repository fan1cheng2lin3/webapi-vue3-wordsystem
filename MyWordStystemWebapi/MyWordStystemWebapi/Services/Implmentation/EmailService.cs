using System.Net.Mail;
using System.Net;

namespace MyWordStystemWebapi.Services.Implmentation
{
    public class EmailService
    {

        private readonly SmtpClient _smtpClient;
        private readonly string _mailFromAddress;

        public EmailService(string mailFromAddress, bool useSsl, string username, string password, string serverName, int serverPort)
        {
            _mailFromAddress = mailFromAddress;
            _smtpClient = new SmtpClient
            {
                EnableSsl = useSsl,
                Host = serverName,
                Port = serverPort,
                Credentials = new NetworkCredential(username, password)
            };
        }

        public void SendEmail(string toAddress, string subject, string passwordReset)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_mailFromAddress),
                Subject = subject,
                Body = $"您的密码已重置为:{passwordReset}"
            };
            mailMessage.To.Add(toAddress);
            _smtpClient.Send(mailMessage);
        }

    }
}
