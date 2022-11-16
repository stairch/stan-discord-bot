using System.Text.Json;
using NLog;

namespace StanDatabase
{
    public static class StanDatabaseConfigLoader
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private const string CONFIG_FILE = "stanDatabaseConfig.json";

        private static StanDatabaseConfig _config;

        public static StanDatabaseConfig Get()
        {
            if (_config == null)
            {
                LoadConfig();
            }
            return _config;
        }

        private static void LoadConfig()
        {
            string jsonString;
            try
            {
                jsonString = File.ReadAllText($"./{CONFIG_FILE}");
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
                _config = JsonSerializer.Deserialize<StanDatabaseConfig>(jsonString, options);
            }
            catch (Exception ex)
            {
                string errorMessage = $"JSON deserialization failed!\n{ex.Message}";
                _logger.Error(errorMessage);
                Console.Error.WriteLine(errorMessage);
                throw;
            }
        }
    }
}
