using StanDatabase;
using StanScript;

namespace StanScripts
{
    internal static class LoadModules
    {
        public static void LoadModulesFromFile(String filePath)
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
            while (!reader.EndOfStream)
            {
                string[] values = CsvHelper.GetCsvValuesOnNextLine(reader);

                string moduleShortname = values[moduleShortnameIndex].Trim();
                if (!Student.IsStudentEmailFormatValid(moduleShortname))
                {
                    Console.Error.WriteLine($"Module format is wrong! No changes made! Fix it and retry the whole file. Module: {moduleShortname}");
                    return;
                }

                string moduleFullname = values[moduleFullnameIndex];

                Module module = new Module();
                currentModules.Add(module);
                Console.WriteLine(module);
            }

            //InsertStudentsIntoDb(currentModules);

            if (ShouldOldModulesBeRemoved())
            {
                //RemoveOldModules(currentModules);
            }
        }

        private static void InsertStudentsIntoDb(IList<Module> currentModules)
        {
            // TODO
            throw new NotImplementedException();
        }

        private static bool ShouldOldModulesBeRemoved()
        {
            // TODO
            string question = "";
            return ConsoleHelper.YesNoQuestion(question);
        }

        private static void RemoveOldModules(IList<Module> currentModules)
        {
            // TODO
            throw new NotImplementedException();
        }
    }
}
