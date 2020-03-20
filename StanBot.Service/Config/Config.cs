using Newtonsoft.Json;

namespace StanBot.Service.Config
{
    public class Config
    {
        [JsonConstructor]
        public Config(string fromMailAdress, string appId, string[] scopes, string discordApplicationToken)
        {
            this.FromMailAdress = fromMailAdress;
            this.AppId = appId;
            this.Scopes = scopes;
            this.DiscordApplicationToken = discordApplicationToken;
        }

        public string AppId { get; }

        public string[] Scopes { get; }

        public string DiscordApplicationToken { get; }

        public string FromMailAdress { get; }
    }
}