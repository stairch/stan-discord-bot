using NLog;
using StanDatabase;
using StanDatabase.Models;
using StanDatabase.Repositories;
using StanDatabase.Util;
using StanScript;

namespace StanScripts
{
    public class LoadModules
    {
        public const string COMMAND_NAME = "loadModules";

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IModuleRepository _moduleRepository;

        public LoadModules(IModuleRepository moduleRepository)
        {
            _moduleRepository = moduleRepository;
        }

        public void LoadModulesFromFile(String filePath)
        {
            if (!File.Exists(filePath))
            {
                string error = $"Error: File not found! Check your path. Path: {filePath}";
                _logger.Error(error);
                Console.Error.WriteLine(error);
                return;
            }

            StreamReader reader = new StreamReader(File.OpenRead(filePath));

            IList<string> columnNames = CsvHelper.GetCsvValuesOnNextLine(reader).ToList();
            string columnInfo = $"Columns in file: {String.Join(", ", columnNames)}";
            _logger.Info(columnInfo);
            Console.WriteLine(columnInfo);

            int moduleShortnameColumnIndex = columnNames.IndexOf(StanDatabaseConfigLoader.Get().ModuleShortnameColumnNameInCsv);
            int moduleFullnameColumnIndex = columnNames.IndexOf(StanDatabaseConfigLoader.Get().ModuleFullnameColumnNameInCsv);

            _logger.Info($"{nameof(moduleShortnameColumnIndex)}: {moduleShortnameColumnIndex} | {nameof(moduleFullnameColumnIndex)}: {moduleFullnameColumnIndex}");

            IList<Module> currentModules = new List<Module>();
            while (!reader.EndOfStream)
            {
                string[] values = CsvHelper.GetCsvValuesOnNextLine(reader);

                string moduleOccassionNumber = values[moduleShortnameColumnIndex];
                if (!ModuleUtil.IsModuleOccasionNumberValid(moduleOccassionNumber))
                {
                    string errorMessage = $"Module format is wrong! No changes made! Fix it and retry the whole file. Module: {moduleOccassionNumber}";

                    if (String.IsNullOrEmpty(moduleOccassionNumber))
                    {
                        errorMessage += " | Since this module name is empty, you may have an empty line at the end of your file.";
                    }
                    _logger.Error(errorMessage);
                    Console.Error.WriteLine(errorMessage);
                    return;
                }

                moduleOccassionNumber = moduleOccassionNumber.Trim();
                string moduleShortname = ModuleUtil.ExtractModuleShortname(moduleOccassionNumber);
                string moduleFullname = values[moduleFullnameColumnIndex].Trim();

                Module module = new Module();
                module.ChannelName = moduleShortname;
                module.FullModuleName = moduleFullname;

                currentModules.Add(module);
            }

            currentModules = currentModules.DistinctBy(m => m.ChannelName).ToList();

            _moduleRepository.InsertMultiple(currentModules);

            if (ShouldOldModulesBeRemoved())
            {
                _moduleRepository.RemoveOld(currentModules);
            }
        }

        private bool ShouldOldModulesBeRemoved()
        {
            string question = "Should modules not contained in the list be removed? ({ConsoleHelper.YesAnswer}/{ConsoleHelper.NoAnswer})" +
                $"\nAnswering with {ConsoleHelper.YesAnswer} removes the other module channels.\"";
            return ConsoleHelper.YesNoQuestion(question);
        }
    }
}
