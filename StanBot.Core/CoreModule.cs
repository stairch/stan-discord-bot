namespace StanBot.Core
{
    using Ninject.Modules;

    using StanBot.Core.MailService;
    using StanBot.Core.MessageProcessors;

    public class CoreModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<DiscordClient>().ToSelf().InSingletonScope();
            this.Bind<IMailService>().To<MailService.MailService>().InSingletonScope();
            this.Bind<VerificationCodeManager>().ToSelf().InSingletonScope();
            this.Bind<Communicator>().ToSelf().InSingletonScope();
            this.Bind<IMessageProcessor>().To<AuthenticationMessageProcessor>().Named("MessageReceived");
            this.Bind<IMessageProcessor>().To<VerificationCodeMessageProcessor>().Named("MessageReceived");
            this.Bind<IMessageProcessor>().To<AnnouncementCommandMessageProcessor>().Named("MessageReceived");
        }
    }
}