namespace StanBot.Service.Configuration
{
    using Newtonsoft.Json;

    public class Config
    {
        [JsonConstructor]
        public Config(string fromMailAddress, string fromName, string appId, string[] scopes, string discordApplicationToken, string guildName, string announcementRoleName)
        {
            this.FromMailAddress = fromMailAddress;
            this.FromName = fromName;
            this.AppId = appId;
            this.Scopes = scopes;
            this.DiscordApplicationToken = discordApplicationToken;
            this.GuildName = guildName;
            this.AnnouncementRoleName = announcementRoleName;
        }

        public string AppId { get; }

        public string[] Scopes { get; }

        public string DiscordApplicationToken { get; }

        public string FromMailAddress { get; }

        public string FromName { get; }
        
        public string GuildName { get; }
        
        public string AnnouncementRoleName { get; }

        public override string ToString()
        {
            return $"{nameof(this.AppId)}: {this.AppId}\n"
                + $"{nameof(this.Scopes)}: {string.Join(", ",this.Scopes)}\n"
                + $"{nameof(this.DiscordApplicationToken)}: {this.DiscordApplicationToken}\n"
                + $"{nameof(this.FromMailAddress)}: {this.FromMailAddress}\n"
                + $"{nameof(this.FromName)}: {this.FromName}\n"
                + $"{nameof(this.GuildName)}: {this.GuildName}\n"
                + $"{nameof(this.AnnouncementRoleName)}: {this.AnnouncementRoleName}";
        }
    }
}