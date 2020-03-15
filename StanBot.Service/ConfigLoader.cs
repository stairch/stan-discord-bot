namespace StanBot.Service
{
    using System.IO;
    using System.Security;

    using Newtonsoft.Json;

    public class ConfigLoader
    {
        public Config LoadConfigFromFile(string pathToFile = "./stan.config")
        {
            string readAllText = File.ReadAllText(pathToFile);
            return JsonConvert.DeserializeObject<Config>(readAllText);
        }
    }

    public class Config
    {
        [JsonConstructor]
        public Config(string fromMailAdress, string smtpServer, int smtpPort, string smtpUsername, string smtpPassword, string discordApplicationToken)
        {
            this.FromMailAdress = fromMailAdress;
            this.SmtpServer = smtpServer;
            this.SmtpPort = smtpPort;
            this.SmtpUsername = smtpUsername;
            this.DiscordApplicationToken = discordApplicationToken;

            foreach (char c in smtpPassword)
            {
                this.SmtpPassword.AppendChar(c);
            }
        }

        public string SmtpServer { get; }

        public int SmtpPort { get; }

        public string SmtpUsername { get; }

        public SecureString SmtpPassword { get; } = new SecureString();

        public string DiscordApplicationToken { get; }

        public string FromMailAdress { get; }
    }
}