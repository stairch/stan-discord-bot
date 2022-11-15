using System.Text.Json;

namespace StanBot
{
    public class StanBotConfigLoader
    {
        // TODO: add logger
        private const string CONFIG_FILE = "stanBotConfig.json";
        private static BotConfig _botConfig;

        public static void LoadConfig()
        {
            string jsonString = File.ReadAllText($"./{CONFIG_FILE}");

            try
            {
                _botConfig = JsonSerializer.Deserialize<BotConfig>(jsonString);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("JSON deserialization failed!");
                Console.Error.WriteLine(ex.Message);
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
