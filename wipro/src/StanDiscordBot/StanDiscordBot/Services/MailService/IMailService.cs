namespace StanBot.Services.MailService
{
    public interface IMailService
    {
        Task SendMailToAsync(string mailAdress, string subject, string messageBody);

        Task InitializeAsync(string fromMailAddress, string fromName, string appId, string[] scopes);
    }
}
