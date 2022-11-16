using NLog;

namespace StanScript
{
    public static class ConsoleHelper
    {
        public const string YesAnswer = "y";

        public const string NoAnswer = "n";

        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public static bool YesNoQuestion(string question)
        {
            string answer;
            do
            {
                _logger.Info($"{nameof(YesNoQuestion)} - {nameof(question)}: {question}");
                Console.WriteLine(question);
                answer = Console.ReadLine();
                _logger.Info($"{nameof(answer)} from user: {answer}");
                if (answer != null)
                {
                    answer = answer.Trim().ToLower();
                    if (answer != YesAnswer && answer != NoAnswer)
                    {
                        _logger.Warn($"{nameof(answer)} from user is invalid: {answer}");
                        Console.Error.WriteLine("Unknown input! Try again.");
                    }
                }
            } while (answer == null || (answer != YesAnswer && answer != NoAnswer));
            return answer == YesAnswer;
        }
    }
}
