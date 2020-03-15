namespace StanBot.Core
{
    using System;
    using System.Net;
    using System.Net.Mail;
    using System.Security;
    using System.Threading.Tasks;

    public class MailService : IMailService
    {
        private SmtpClient smtpClient;

        private string fromMailAdress;

        public async Task SendMailToAsync(string mailAdress, string subject, string messageBody)
        {
            try
            {
                MailMessage mailMessage = new MailMessage(this.fromMailAdress, mailAdress, subject, messageBody);
                await this.smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
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