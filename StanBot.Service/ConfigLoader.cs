namespace StanBot.Service
{
    using System;
    using System.IO;

    using Newtonsoft.Json;

    public class ConfigLoader
    {
        public Config LoadConfigFromFile(string pathToFile = "./stan.config")
        {
            string readAllText = File.ReadAllText(pathToFile);
            try
            {
                return JsonConvert.DeserializeObject<Config>(readAllText);
            }
            catch (JsonException e)
            {
                Console.WriteLine("Json deserialization failed. You probably used the wrong format for the config file. It has to be a json file and needs the following fields: FromMailAdress: string, SmtpServer: string, SmtpPort: int, SmtpUsername: string, SmtpPassword: string and DiscordApplicationToken: string");
                Console.WriteLine(e);
                throw;
            }
        }
    }
}