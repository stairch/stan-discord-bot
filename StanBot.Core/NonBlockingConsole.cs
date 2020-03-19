namespace StanBot.Service
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;

    public static class NonBlockingConsole
    {
        private static readonly BlockingCollection<string> MessageQueue = new BlockingCollection<string>();

        static NonBlockingConsole()
        {
            Thread thread = new Thread(
                                () =>
                                    {
                                        while (true)
                                        {
                                            Console.WriteLine(MessageQueue.Take());
                                        }
                                    }) { IsBackground = true };
            thread.Start();
        }

        public static void WriteLine(string value)
        {
            MessageQueue.Add(value);
        }
    }
}