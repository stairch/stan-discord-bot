using Microsoft.Graph;
using NLog;
using StanBot.Services.ErrorNotificactionService;

namespace StanBot.Services.MailService
{
    public class MailService : IMailService
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly MailErrorNotificationService _errorNotificationService;

        private string _fromMailAddress;
        private string _fromName;
        private GraphServiceClient _graphServiceClient;

        public MailService(MailErrorNotificationService errorNotificationService)
        {
            _errorNotificationService = errorNotificationService;
        }

        public async Task InitializeAsync(string fromMailAddress, string fromName, string appId, string[] scopes)
        {
            _fromMailAddress = fromMailAddress;
            _fromName = fromName;

            DeviceCodeAuthProvider authProvider = new DeviceCodeAuthProvider(appId, scopes);
            await authProvider.GetAccessToken();

            _graphServiceClient = new GraphServiceClient(authProvider);
        }

        public async Task SendMailToAsync(string mailAdress, string subject, string messageBody, int importance = 1)
        {
            try
            {
                Message message = new Message
                {
                    Subject = subject,
                    Body = new ItemBody { ContentType = BodyType.Text, Content = messageBody },
                    ToRecipients = new List<Recipient> { new Recipient { EmailAddress = new EmailAddress { Address = mailAdress } } },
                    Sender = new Recipient { EmailAddress = new EmailAddress { Address = _fromMailAddress, Name = _fromName } },
                    From = new Recipient { EmailAddress = new EmailAddress { Address = _fromMailAddress, Name = _fromName } },
                    Importance = (Importance) importance
                };

                await _graphServiceClient.Me.SendMail(message).Request().PostAsync();
                _logger.Info($"E-mail successfully send to {mailAdress}");
            }
            catch (Exception ex)
            {
                _errorNotificationService.SendMailErrorToAdmins(ex, "MailService");
                _logger.Error($"Could not send E-Mail. Stracktrace: {ex.Message}");
                throw;
            }
        }
    }
}
