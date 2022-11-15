using System.Text.Json;

namespace StanDatabase
{
    public static class StanDatabaseConfigLoader
    {
        // TODO: add logger
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
            string jsonString = File.ReadAllText($"./{CONFIG_FILE}");
            try
            {
                _config = JsonSerializer.Deserialize<StanDatabaseConfig>(jsonString);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("JSON deserialization failed!");
                Console.Error.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
