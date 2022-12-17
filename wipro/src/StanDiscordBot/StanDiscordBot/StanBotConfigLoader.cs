using NLog;
using System;
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
            _logger.Info($"Execute {nameof(LoadConfig)}");
            string jsonString;
            try
            {
	           	//string path = Path.Combine(_appEnvironment.ContentRootPath, CONFIG_FILE);
            	string path = $"/home/localadmin/stan-discord-bot/wipro/src/StanDiscordBot/artifacts/{CONFIG_FILE}";
            	Console.WriteLine("Bot Config Path " + path);
            	jsonString = File.ReadAllText(path);
                //jsonString = File.ReadAllText($"./{CONFIG_FILE}");
            }
            catch (Exception ex)
            {
                string errorMessage = $"Couldn't read config file!\n{ex.Message}";
                _logger.Error(errorMessage);
                Console.Error.WriteLine(errorMessage);
                throw;
            }

            try
            {
                var options = new JsonSerializerOptions
                {
                    ReadCommentHandling = JsonCommentHandling.Skip
                };
                _botConfig = JsonSerializer.Deserialize<BotConfig>(jsonString, options);
            }
            catch (Exception ex)
            {
                string errorMessage = $"JSON deserialization failed!\n{ex.Message}";
                _logger.Error(errorMessage);
                Console.Error.WriteLine(errorMessage);
                throw;
            }
        }

        public static BotConfig Config { get { return _botConfig; } }
    }

    public struct BotConfig
    {
        public string DiscordApplicationToken { get; set; }

        public ulong GuildId { get; set; }

        public string Prefix { get; set; }

        public string FromEmailAddress { get; set; }

        public string FromEmailName { get; set; }

        public string AppId { get; set; }

        public string[] Scopes { get; set; }
    }
}
