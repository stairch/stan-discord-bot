namespace StanBot.Core
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.Graph;

    public class MailService : IMailService
    {
        private string fromMailAdress;

        private GraphServiceClient graphServiceClient;

        public async Task SendMailToAsync(string mailAdress, string subject, string messageBody)
        {
            try
            {
                Message message = new Message
                                      {
                                          Subject = subject,
                                          Body = new ItemBody
                                                     {
                                                         ContentType = BodyType.Text,
                                                         Content = messageBody
                                                     },
                                          ToRecipients = new List<Recipient>
                                                             {
                                                                 new Recipient
                                                                     {
                                                                         EmailAddress = new EmailAddress
                                                                                            {
                                                                                                Address = mailAdress
                                                                                            }
                                                                     }
                                                             }
                                      };
                await this.graphServiceClient.Me.SendMail(message).Request().PostAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task Initialize(string fromMailAdress, string appId, string[] scopes)
        {
            DeviceCodeAuthProvider authProvider = new DeviceCodeAuthProvider(appId, scopes);

            await authProvider.GetAccessToken();

            this.graphServiceClient = new GraphServiceClient(authProvider);
        }
    }
}