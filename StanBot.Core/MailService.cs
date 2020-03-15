namespace StanBot.Core
{
    using System.Net;
    using System.Net.Mail;
    using System.Security;

    public class MailService
    {
        private SmtpClient smtpClient;

        private string fromMailAdress;

        public void SendMailTo(string mailAdress, string subject, string messageBody)
        {
            MailMessage mailMessage = new MailMessage(this.fromMailAdress, mailAdress, subject, messageBody);
            this.smtpClient.Send(mailMessage);
        }

        public void Initialize(string fromMailAdress, string smtpServer, int smtpPort, string smtpUsername, SecureString smtpPassword)
        {
            this.smtpClient = new SmtpClient(smtpServer, smtpPort)
                                  {
                                      Credentials = new NetworkCredential(smtpUsername, smtpPassword)
                                  };
            this.fromMailAdress = fromMailAdress;
        }
    }
}