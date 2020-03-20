namespace StanBot.Service
{
    using Ninject.Modules;

    using StanBot.Service.Configuration;

    public class ServiceModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<ServiceManager>().ToSelf();
            this.Bind<ConfigLoader>().ToSelf();
        }
    }
}