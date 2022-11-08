using StanDatabase;
using StanDatabase.DataAccessLayer;
using StanDatabase.Models;
using StanDatabase.Repositories;
using StanDatabase.Util;
using StanScript;

namespace StanScripts
{
    // TODO: add logging
    public class LoadModules
    {
        private readonly IModuleRepository _moduleRepository;

        public LoadModules(IModuleRepository moduleRepository)
        {
            _moduleRepository = moduleRepository;
        }

        public void LoadModulesFromFile(String filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.Error.WriteLine("Error: File not found! Check your path.");
                return;
            }

            StreamReader reader = new StreamReader(File.OpenRead(filePath));

            IList<string> columnNames = CsvHelper.GetCsvValuesOnNextLine(reader).ToList();
            Console.WriteLine($"Columns in file: {String.Join(", ", columnNames)}");

            int moduleShortnameIndex = columnNames.IndexOf(StanSettings.ModuleShortnameInCsv);
            int moduleFullnameIndex = columnNames.IndexOf(StanSettings.ModuleFullnameInCsv);

            Console.WriteLine($"{nameof(moduleShortnameIndex)}: {moduleShortnameIndex} | {nameof(moduleFullnameIndex)}: {moduleFullnameIndex}");

            IList<Module> currentModules = new List<Module>();
            // TODO: filter module duplicates (check long name)
            while (!reader.EndOfStream)
            {
                string[] values = CsvHelper.GetCsvValuesOnNextLine(reader);

                string moduleOccassionNumber = values[moduleShortnameIndex].Trim();
                if (!ModuleUtil.IsModuleOccasionNumberValid(moduleOccassionNumber))
                {
                    string errorMessage = $"Module format is wrong! No changes made! Fix it and retry the whole file. Module: {moduleOccassionNumber}";

                    if (String.IsNullOrEmpty(moduleOccassionNumber))
                    {
                        errorMessage += " | Since this module name is empty, you may have an empty line at the end of your file.";
                    }
                    Console.Error.WriteLine(errorMessage);
                    return;
                }

                string moduleShortname = ModuleUtil.ExtractModuleShortname(moduleOccassionNumber);
                string moduleFullname = values[moduleFullnameIndex];

                // TODO: use injection
                IDiscordCategoryRepository discordCategoryRepository = new DiscordCategoryRepository();
                Module module = new Module(moduleShortname, moduleFullname, discordCategoryRepository.GetCategoryWithChannelCapacity());
                currentModules.Add(module);
                Console.WriteLine(module);
            }

            _moduleRepository.InsertMultiple(currentModules);

            if (ShouldOldModulesBeRemoved())
            {
                _moduleRepository.RemoveOld(currentModules);
            }
        }

        private bool ShouldOldModulesBeRemoved()
        {
            // TODO
            string question = "";
            return ConsoleHelper.YesNoQuestion(question);
        }
    }
}
