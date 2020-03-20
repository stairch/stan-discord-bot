using System;

namespace StanBot.Core
{
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    using File = System.IO.File;

    public class NonBlockingLogger
    {
        private static readonly NonBlockingLogger Logger = new NonBlockingLogger();

        private NonBlockingLogger()
        { }
        
        public static async Task DebugAsync(string value)
        {
            await Logger.Log($"{DateTime.Now} DEBUG {value}\n");
        }

        public static async Task InfoAsync(string value)
        {
            await Logger.Log($"{DateTime.Now} INFO {value}\n");
        }

        public static async Task WarnAsync(string value)
        {
            await Logger.Log($"{DateTime.Now} WARN {value}\n");
        }

        public static async Task ErrorAsync(string value)
        {
            await Logger.Log($"{DateTime.Now} ERROR {value}\n");
        }

        public static async Task FatalAsync(string value)
        {
            await Logger.Log($"{DateTime.Now} FATAL {value}\n");
        }

        private async Task Log(string message)
        {
            using (FileStream file = File.Open("./stan.log", FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                byte[] bytes = Encoding.ASCII.GetBytes(message);
                await file.WriteAsync(bytes, 0, bytes.Length);
            }
        }
    }
}