namespace StanBot.Core
{
    using Nito.AsyncEx;
    using System;
    using System.Collections.Concurrent;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    public class NonBlockingLogger
    {
        private static readonly NonBlockingLogger Logger = new NonBlockingLogger();

        private readonly ConcurrentQueue<string> messageQueue;

        private readonly AsyncAutoResetEvent asyncAutoResetEvent;

        private NonBlockingLogger()
        {
            this.messageQueue = new ConcurrentQueue<string>();
            this.asyncAutoResetEvent = new AsyncAutoResetEvent();
            this.LogToFile();
        }
        
        public static void Debug(string value)
        {
            Logger.Log("DEBUG", value);
        }

        public static void Info(string value)
        {
            Logger.Log("INFO", value);
        }

        public static void Warn(string value)
        {
            Logger.Log("WARN", value);
        }

        public static void Error(string value)
        {
            Logger.Log("ERROR", value);
        }

        public static void Fatal(string value)
        {
            Logger.Log("FATAL", value);
        }

        private void Log(string level, string value)
        {
            this.messageQueue.Enqueue($"{DateTime.Now.ToString(CultureInfo.InvariantCulture)} {level} {value}\n\r");
            this.asyncAutoResetEvent.Set();
        }

        private async Task LogToFile()
        {
            while (true)
            {
                await this.asyncAutoResetEvent.WaitAsync().ConfigureAwait(false);
                using (FileStream file = File.Open("./stan.log", FileMode.Append, FileAccess.Write))
                {
                    if (this.messageQueue.TryDequeue(out string logMessage))
                    {
                        byte[] bytes = Encoding.ASCII.GetBytes(logMessage);
                        await file.WriteAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
                    }
                }
            }
        }
    }
}