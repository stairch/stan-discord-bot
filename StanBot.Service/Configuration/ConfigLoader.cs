namespace StanBot.Service.Configuration
{
    using System;
    using System.IO;

    using Newtonsoft.Json;

    public class ConfigLoader
    {
        public Config LoadConfigFromFile(string pathToFile = "./stan.json")
        {
            string readAllText = File.ReadAllText(pathToFile);
            try
            {
                return JsonConvert.DeserializeObject<Config>(readAllText);
            }
            catch (JsonException e)
            {
                Console.WriteLine("Json deserialization failed. You probably used the wrong format for the config file. It has to be a json file and needs the following fields: FromMailAdress: string, AppId: string, Scopes: string[], DiscordApplicationToken: string");
                Console.WriteLine(e);
                throw;
            }
        }
    }
}