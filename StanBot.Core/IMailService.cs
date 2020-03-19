namespace StanBot.Core
{
    using System.Threading.Tasks;

    public interface IMailService
    {
        Task SendMailToAsync(string mailAdress, string subject, string messageBody);

        Task Initialize(string fromMailAdress, string appId, string[] scopes);
    }
}