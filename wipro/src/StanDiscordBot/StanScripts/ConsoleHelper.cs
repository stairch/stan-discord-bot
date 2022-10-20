namespace StanScript
{
    public static class ConsoleHelper
    {
        public const string YesAnswer = "y";

        public const string NoAnswer = "n";

        public static bool YesNoQuestion(string question)
        {
            string answer;
            do
            {
                Console.WriteLine(question);
                answer = Console.ReadLine().ToLower();
                if (answer != YesAnswer && answer != NoAnswer)
                {
                    Console.Error.WriteLine("Unknown input! Try again.");
                }
            } while (answer != YesAnswer && answer != NoAnswer);

            return answer == YesAnswer;
        }
    }
}
