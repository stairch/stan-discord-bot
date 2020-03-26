namespace StanBot.Core.MailService
{
    using System.Threading.Tasks;

    public interface IMailService
    {
        Task SendMailToAsync(string mailAdress, string subject, string messageBody);

        Task Initialize(string fromMailAddress, string fromName, string appId, string[] scopes);
    }
}