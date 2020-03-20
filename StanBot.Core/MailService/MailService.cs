namespace StanBot.Core.MailService
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.Graph;

    public class MailService : IMailService
    {
        private string fromMailAdress;

        private GraphServiceClient graphServiceClient;

        private string fromName;

        public async Task SendMailToAsync(string mailAdress, string subject, string messageBody)
        {
            try
            {
                Message message = new Message();
                message.Subject = subject;
                message.Body = new ItemBody { ContentType = BodyType.Text, Content = messageBody };
                message.ToRecipients = new List<Recipient> { new Recipient { EmailAddress = new EmailAddress { Address = mailAdress } } };
                message.Sender = new Recipient { EmailAddress = new EmailAddress { Address = this.fromMailAdress, Name = this.fromName } };
                message.From = new Recipient { EmailAddress = new EmailAddress { Address = this.fromMailAdress, Name = this.fromName } };
                await this.graphServiceClient.Me.SendMail(message).Request().PostAsync();
            }
            catch (Exception e)
            {
                await NonBlockingLogger.ErrorAsync($"{e.Message}\nStacktrace: {e.StackTrace}");
            }
        }

        public async Task Initialize(string fromMailAdress, string fromName, string appId, string[] scopes)
        {
            this.fromMailAdress = fromMailAdress;
            this.fromName = fromName;
            DeviceCodeAuthProvider authProvider = new DeviceCodeAuthProvider(appId, scopes);

            await authProvider.GetAccessToken();

            this.graphServiceClient = new GraphServiceClient(authProvider);
        }
    }
}