using NLog;
using System.Linq;

namespace StanScripts
{
    public static class Program
    {
        private static Logger logger;

        /// <summary>
        ///
        /// </summary>
        /// <param name="args">
        /// The first argument describes the command that should be executed.
        /// The next argument(s) are additional information for that command.
        /// </param>
        public static void Main(string[] args)
        {
            logger = LogManager.GetCurrentClassLogger();
            if (args.Length == 0)
            {
                logger.Info("No arguments given");
                PrintHelp();
                return;
            }

            Console.WriteLine($"arguments: {String.Join(", ", args)} | length: {args.Length}");
            string commandName = args[0];
            logger.Info($"Selected {nameof(commandName)}: {commandName}");
            if (commandName == "printHelp")
            {
                PrintHelp();
            }
            else if (commandName == "loadStudents")
            {
                if (args.Length == 2)
                {
                    LoadStudents.LoadStudentsFromFile(args[1]);
                }
                else
                {
                    string errorMessage = $"Wrong number of arguments given for {commandName}!";
                    logger.Error(errorMessage);
                    Console.Error.WriteLine($"{errorMessage} Check your command and try again. This is most likely an issue from the user.");
                    PrintHelp();
                }
            }
            else if (commandName == "loadModules")
            {
                if (args.Length == 2)
                {
                    LoadModules.LoadModulesFromFile(args[1]);
                }
                else
                {
                    string errorMessage = $"Wrong number of arguments given for {commandName}!";
                    logger.Error(errorMessage);
                    Console.Error.WriteLine($"{errorMessage} Check your command and try again. This is most likely an issue from the user.");
                    PrintHelp();
                }
            }
        }

        public static void PrintHelp()
        {
            logger.Debug($"{nameof(PrintHelp)} is called");
            // TODO: describe the commands here
            Console.WriteLine("This program is for the Stan Discord bot from STAIR");
        }
    }
}
