using NLog;
using System.Text.Json;

namespace StanBot
{
    public class StanBotConfigLoader
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private const string CONFIG_FILE = "stanBotConfig.json";
        private static BotConfig _botConfig;

        public static void LoadConfig()
        {
            string jsonString = File.ReadAllText($"./{CONFIG_FILE}");

            try
            {
                _logger.Info($"Execute {nameof(LoadConfig)}");
                _botConfig = JsonSerializer.Deserialize<BotConfig>(jsonString);
            }
            catch (Exception ex)
            {
                string errorMessage = $"JSON deserialization failed!\n{ex.Message}";
                _logger.Error(errorMessage);
                Console.Error.WriteLine(errorMessage);
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
