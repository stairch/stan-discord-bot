using Microsoft.Graph;

namespace StanBot.Services.MailService
{
    public class MailService : IMailService
    {
        private string _fromMailAddress;
        private string _fromName;

        private GraphServiceClient _graphServiceClient;

        public async Task InitializeAsync(string fromMailAddress, string fromName, string appId, string[] scopes)
        {
            _fromMailAddress = fromMailAddress;
            _fromName = fromName;

            DeviceCodeAuthProvider authProvider = new DeviceCodeAuthProvider(appId, scopes);
            await authProvider.GetAccessToken();

            _graphServiceClient = new GraphServiceClient(authProvider);
        }

        public async Task SendMailToAsync(string mailAdress, string subject, string messageBody)
        {
            try
            {
                Message message = new Message
                {
                    Subject = subject,
                    Body = new ItemBody { ContentType = BodyType.Text, Content = messageBody },
                    ToRecipients = new List<Recipient> { new Recipient { EmailAddress = new EmailAddress { Address = mailAdress } } },
                    Sender = new Recipient { EmailAddress = new EmailAddress { Address = _fromMailAddress, Name = _fromName } },
                    From = new Recipient { EmailAddress = new EmailAddress { Address = _fromMailAddress, Name = _fromName } }
                };

                await _graphServiceClient.Me.SendMail(message).Request().PostAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not send E-Mail.\n{ex}");
            }
        }
    }
}
