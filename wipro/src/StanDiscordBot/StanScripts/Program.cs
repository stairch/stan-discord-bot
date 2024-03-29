﻿using NLog;
using StanDatabase.DataAccessLayer;
using StanDatabase.Repositories;

namespace StanScripts
{
    public static class Program
    {
        public const string HELP_COMMAND_NAME = "help";

        private static Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///
        /// </summary>
        /// <param name="args">
        /// The first argument describes the command that should be executed.
        /// The next argument(s) are additional information for that command.
        /// </param>
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                _logger.Info("No arguments given.");
                PrintHelp();
                return;
            }

            _logger.Info($"arguments: {String.Join(", ", args)} | length: {args.Length}");
            string commandName = args[0];
            _logger.Info($"Selected {nameof(commandName)}: {commandName}");
            if (commandName == HELP_COMMAND_NAME)
            {
                PrintHelp();
            }
            else if (commandName == LoadStudents.COMMAND_NAME)
            {
                if (args.Length == 2)
                {
                    LoadStudents loadStudents = new LoadStudents(new StudentRepository(), new HouseRepository());
                    loadStudents.LoadStudentsFromFile(args[1]);
                }
                else
                {
                    string errorMessage = $"Wrong number of arguments given for {commandName}!";
                    _logger.Error(errorMessage);
                    Console.Error.WriteLine($"{errorMessage} Check your command and try again. This is most likely an issue from the user.");
                    PrintHelp();
                }
            }
            else if (commandName == LoadModules.COMMAND_NAME)
            {
                if (args.Length == 2)
                {
                    LoadModules loadModules = new LoadModules(new ModuleRepository(new DiscordCategoryRepository()));
                    loadModules.LoadModulesFromFile(args[1]);
                }
                else
                {
                    string errorMessage = $"Wrong number of arguments given for {commandName}!";
                    _logger.Error(errorMessage);
                    Console.Error.WriteLine($"{errorMessage} Check your command and try again. This is most likely an issue from the user.");
                    PrintHelp();
                }
            }
            else if (commandName == "dbtest")
            {
                Console.WriteLine("Executing database test...");
                try
                {
                    IStudentRepository studentRepository = new StudentRepository();
                    Console.WriteLine($"Found students: {studentRepository.GetCurrentStudents().Count}");
                    Console.WriteLine($"Connection worked");
                }
                catch (System.Exception ex)
                {
                    Console.Error.WriteLine($"Exception thrown during the DB test. Errormessage: {ex.Message}\nInnerMessage: {ex.InnerException}\nStack Trace: {ex.StackTrace}");
                }
            }
            else
            {
                Console.Error.WriteLine($"Command \"{commandName}\" is unknown!");
            }
        }

        public static void PrintHelp()
        {
            _logger.Debug($"{nameof(PrintHelp)} is called");

            string projectName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
            string helpInformation = "This program is for the Stan Discord Bot from STAIR (HSLU)." +
                $"\n\nUse {HELP_COMMAND_NAME} to get this information." +
                $"\nExample (Windows): {projectName}.exe {HELP_COMMAND_NAME}" +
                $"\nExample (Linux): ./{projectName} {HELP_COMMAND_NAME}" +
                $"\n\nUse {LoadModules.COMMAND_NAME} to load the modules into the database and creating the channels on the discord." +
                $"\nAdd the module file path as an argument" +
                $"\nExample (Windows): {projectName}.exe {LoadModules.COMMAND_NAME} C:/temp/ModulesHS22.csv" +
                $"\nExample (Linux): ./{projectName} {LoadModules.COMMAND_NAME} ~/ModulesHS22.csv" +
                $"\n\nUse {LoadStudents.COMMAND_NAME} to load the current students into the database." +
                $"\nAdd the student file path as an argument" +
                $"\nExample (Windows): {projectName}.exe {LoadStudents.COMMAND_NAME} C:/temp/StudentsHS22.csv" +
                $"\nExample (Linux): ./{projectName} {LoadStudents.COMMAND_NAME} ~/ModulesHS22.csv";
            Console.WriteLine(helpInformation);
        }
    }
}
