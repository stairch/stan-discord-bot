using System.Linq;

namespace StanScripts
{
    public static class Program
    {
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
                PrintHelp();
                return;
            }

            Console.WriteLine($"arguments: {String.Join(", ", args)} | length: {args.Length}");
            string commandName = args[0];
            Console.WriteLine($"Selected task: {commandName}");
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
                    Console.Error.WriteLine("Wrong number of arguments! Check your command and try again. This is most likely an issue from the user.");
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
                    Console.Error.WriteLine("Wrong number of arguments! Check your command and try again. This is most likely an issue from the user.");
                }
            }
        }

        public static void PrintHelp()
        {
            // TODO: describe the commands here
            Console.WriteLine("This program is for the Stan discord bot from STAIR");
        }
    }
}
