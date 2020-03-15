namespace StanBot.Core
{
    using System.Security;
    using System.Threading.Tasks;

    public interface IMailService
    {
        Task SendMailToAsync(string mailAdress, string subject, string messageBody);

        void Initialize(string fromMailAdress, string smtpServer, int smtpPort, string smtpUsername, SecureString smtpPassword);
    }
}