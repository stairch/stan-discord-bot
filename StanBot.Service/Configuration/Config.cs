namespace StanBot.Service.Configuration
{
    using Newtonsoft.Json;

    public class Config
    {
        [JsonConstructor]
        public Config(string fromMailAdress, string fromName, string appId, string[] scopes, string discordApplicationToken)
        {
            this.FromMailAdress = fromMailAdress;
            this.FromName = fromName;
            this.AppId = appId;
            this.Scopes = scopes;
            this.DiscordApplicationToken = discordApplicationToken;
        }

        public string AppId { get; }

        public string[] Scopes { get; }

        public string DiscordApplicationToken { get; }

        public string FromMailAdress { get; }

        public string FromName { get; }
    }
}