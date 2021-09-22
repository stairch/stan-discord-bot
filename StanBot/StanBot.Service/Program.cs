using Ninject;
using Nito.AsyncEx;
using StanBot.Core;
using System;
using Topshelf;
using Topshelf.Runtime;

namespace StanBot.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            StandardKernel kernel = new StandardKernel();
            kernel.Load("StanBot.*.dll");

            TopshelfExitCode topshelfExitCode = HostFactory.Run(x =>
                {
                    x.Service<ServiceManager>(s =>
                        {
                            s.ConstructUsing(name => kernel.Get<ServiceManager>());
                            s.WhenStarted(tc => AsyncContext.Run(tc.Start));
                            s.WhenStopped(tc => tc.Stop());
                        });

                    // check for permissions
                    x.RunAsLocalSystem();

                    x.UnhandledExceptionPolicy = UnhandledExceptionPolicyCode.LogErrorOnly;
                    x.OnException(HandleException);

                    // is only shown on the server, not in the frontend.
                    x.SetDescription("Discord bot for the STAIR discord to authenticate students from HSLU. May include more features but I don't really know now so this is for everything else that may be integrated and not mentioned here. Bad Heineken!");
                    x.SetDisplayName("STAIR Discord Bot");
                    x.SetServiceName("STAIR Discord Bot");
                });

            int exitCode = (int)Convert.ChangeType(topshelfExitCode, topshelfExitCode.GetTypeCode());
            Environment.ExitCode = exitCode;
        }

        private static void HandleException(Exception exception)
        {
            NonBlockingLogger.Error($"Unhandled exception occured: {exception.Message}\nStacktrace: {exception.StackTrace}");
        }
    }
}
