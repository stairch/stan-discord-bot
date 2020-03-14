namespace StanBot.Service
{
    using Ninject.Modules;

    public class ServiceModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<ServiceManager>().ToSelf();
        }
    }
}