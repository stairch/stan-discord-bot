using Ninject.Modules;
using StanBot.Service.Configuration;

namespace StanBot.Service
{

    public class ServiceModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<ServiceManager>().ToSelf();
            this.Bind<ConfigLoader>().ToSelf();
        }
    }
}