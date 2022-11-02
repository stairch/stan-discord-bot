using System.Text.Json;

namespace StanBot
{
    public class Config
    {
        private const string CONFIG_FILE = "stan.json";
        private static BotConfig _botConfig;

        public static void LoadConfig()
        {
            string jsonString = File.ReadAllText("./" + CONFIG_FILE);

            try
            {
                _botConfig = JsonSerializer.Deserialize<BotConfig>(jsonString);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Json deserialization failed!");
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public static BotConfig Get()
        {
            return _botConfig;
        }
    }

    public struct BotConfig
    {
        public string DiscordApplicationToken { get; set; }

        public string Prefix { get; set; }
    }
}
