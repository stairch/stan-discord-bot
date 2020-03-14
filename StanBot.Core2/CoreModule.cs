namespace StanBot.Core
{
    using Ninject.Modules;

    public class CoreModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<Orchestrator>().ToSelf();
        }
    }
}