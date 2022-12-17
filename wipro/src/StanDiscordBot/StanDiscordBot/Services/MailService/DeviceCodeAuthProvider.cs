using Microsoft.Graph;
using Microsoft.Identity.Client;
using NLog;
using System.Net.Http.Headers;
using Logger = NLog.Logger;

namespace StanBot.Services.MailService
{
    internal class DeviceCodeAuthProvider : IAuthenticationProvider
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IPublicClientApplication _msalClient;
        private readonly string[] _scopes;
        private IAccount? _stanMailAccount;

        internal DeviceCodeAuthProvider(string appId, string[] scopes)
        {
            _scopes = scopes;
            _msalClient = PublicClientApplicationBuilder
                .Create(appId)
                .WithAuthority(AadAuthorityAudience.AzureAdAndPersonalMicrosoftAccount, true)
                .Build();
        }

        internal async Task<string?> GetAccessToken()
        {
            AuthenticationResult result;

            if (_stanMailAccount == null)
            {
                try
                {
                    result = await _msalClient.AcquireTokenWithDeviceCode(_scopes, DeviceCodeResultCallback).ExecuteAsync();
                    _stanMailAccount = result.Account;
                    return result.AccessToken;
                }
                catch (Exception ex)
                {
                    _logger.Error($"Error getting Access Token. Stacktrace: {ex.Message}");
                    return null;
                }
            }

            // If there is an account, call AcquireTokenSilent
            // By doing this, MSAL will refresh the token automatically if
            // it is expired. Otherwise it returns the cached token.
            result = await _msalClient.AcquireTokenSilent(_scopes, _stanMailAccount).ExecuteAsync();
            _logger.Info($"Device Authentication Token silent aquired.");
            return result.AccessToken;
        }

        private Task DeviceCodeResultCallback(DeviceCodeResult deviceCodeResult)
        {
            _logger.Info($"Device Authentication ResultCallback received. Message: {deviceCodeResult.Message}");
            return Task.CompletedTask;
        }

        // This is the required function to implement IAuthenticationProvider
        // The Graph SDK will call this function each time it makes a Graph
        // call.
        public async Task AuthenticateRequestAsync(HttpRequestMessage request)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("bearer", await GetAccessToken());
        }
    }
}
