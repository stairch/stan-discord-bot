namespace StanBot.Service.Configuration
{
    using Newtonsoft.Json;

    public class Config
    {
        [JsonConstructor]
        public Config(string fromMailAddress, string fromName, string appId, string[] scopes, string discordApplicationToken)
        {
            this.FromMailAddress = fromMailAddress;
            this.FromName = fromName;
            this.AppId = appId;
            this.Scopes = scopes;
            this.DiscordApplicationToken = discordApplicationToken;
        }

        public string AppId { get; }

        public string[] Scopes { get; }

        public string DiscordApplicationToken { get; }

        public string FromMailAddress { get; }

        public string FromName { get; }

        public override string ToString()
        {
            return $"{nameof(this.AppId)}: {this.AppId}\n"
                + $"{nameof(this.Scopes)}: {this.Scopes}\n"
                + $"{nameof(this.DiscordApplicationToken)}: {this.DiscordApplicationToken}\n"
                + $"{nameof(this.FromMailAddress)}: {this.FromMailAddress}\n"
                + $"{nameof(this.FromName)}: {this.FromName}";
        }
    }
}