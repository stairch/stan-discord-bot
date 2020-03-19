namespace StanBot.Core
{
    using Ninject.Modules;

    public class CoreModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<DiscordClient>().ToSelf();
            this.Bind<IMailService>().To<MailService>().InSingletonScope();
            this.Bind<VerificationCodeManager>().ToSelf().InSingletonScope();
            this.Bind<Communicator>().ToSelf().InSingletonScope();
            this.Bind<IMessageProcessor>().To<AuthenticationMessageProcessor>().Named("MessageReceived");
            this.Bind<IMessageProcessor>().To<VerirficationCodeMessageProcessor>().Named("MessageReceived");
        }
    }
}