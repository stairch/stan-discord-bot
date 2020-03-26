namespace StanBot.Core.MailService
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    using Microsoft.Graph;
    using Microsoft.Identity.Client;

    public class DeviceCodeAuthProvider : IAuthenticationProvider
    {
        private readonly IPublicClientApplication msalClient;
        private readonly string[] scopes;
        private IAccount userAccount;

        public DeviceCodeAuthProvider(string appId, string[] scopes)
        {
            this.scopes = scopes;

            this.msalClient = PublicClientApplicationBuilder
                .Create(appId)
                .WithAuthority(AadAuthorityAudience.AzureAdAndPersonalMicrosoftAccount, true)
                .Build();
        }

        public async Task<string> GetAccessToken()
        {
            AuthenticationResult result;

            // If there is no saved user account, the user must sign-in
            if (this.userAccount == null)
            {
                try
                {
                    // Invoke device code flow so user can sign-in with a browser
                    result = await this.msalClient.AcquireTokenWithDeviceCode(
                                 this.scopes,
                                 callback =>
                                     {
                                         NonBlockingLogger.Info(callback.Message);
                                         return Task.CompletedTask;
                                     }).ExecuteAsync();

                    this.userAccount = result.Account;
                    return result.AccessToken;
                }
                catch (Exception exception)
                {
                    NonBlockingLogger.Error($"Error getting access token: {exception.Message}");
                    return null;
                }
            }

            // If there is an account, call AcquireTokenSilent
            // By doing this, MSAL will refresh the token automatically if
            // it is expired. Otherwise it returns the cached token.
            result = await this.msalClient.AcquireTokenSilent(this.scopes, this.userAccount).ExecuteAsync();

            return result.AccessToken;
        }

        // This is the required function to implement IAuthenticationProvider
        // The Graph SDK will call this function each time it makes a Graph
        // call.
        public async Task AuthenticateRequestAsync(HttpRequestMessage requestMessage)
        {
            requestMessage.Headers.Authorization =
                new AuthenticationHeaderValue("bearer", await this.GetAccessToken());
        }
    }
}