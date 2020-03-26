namespace StanBot.Core.MailService
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.Graph;

    public class MailService : IMailService
    {
        private string fromMailAddress;

        private GraphServiceClient graphServiceClient;

        private string fromName;

        public async Task SendMailToAsync(string mailAdress, string subject, string messageBody)
        {
            try
            {
                Message message = new Message
                                      {
                                          Subject = subject,
                                          Body = new ItemBody { ContentType = BodyType.Text, Content = messageBody },
                                          ToRecipients = new List<Recipient> { new Recipient { EmailAddress = new EmailAddress { Address = mailAdress } } },
                                          Sender = new Recipient { EmailAddress = new EmailAddress { Address = this.fromMailAddress, Name = this.fromName } },
                                          From = new Recipient { EmailAddress = new EmailAddress { Address = this.fromMailAddress, Name = this.fromName } }
                                      };

                await this.graphServiceClient.Me.SendMail(message).Request().PostAsync();
            }
            catch (Exception e)
            {
                NonBlockingLogger.Error($"{e.Message}\nStacktrace: {e.StackTrace}");
            }
        }

        public async Task Initialize(string fromMailAddress, string fromName, string appId, string[] scopes)
        {
            this.fromMailAddress = fromMailAddress;
            this.fromName = fromName;
            DeviceCodeAuthProvider authProvider = new DeviceCodeAuthProvider(appId, scopes);

            await authProvider.GetAccessToken();

            this.graphServiceClient = new GraphServiceClient(authProvider);
        }
    }
}