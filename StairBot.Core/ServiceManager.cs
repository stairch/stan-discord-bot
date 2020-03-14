namespace StanBot.Service
{
    using System.Threading.Tasks;

    using StanBot.Core;

    public class ServiceManager
    {
        private readonly Orchestrator orchestrator;

        public ServiceManager(Orchestrator orchestrator)
        {
            this.orchestrator = orchestrator;
        }

        public async Task Start()
        {
            await this.orchestrator.LoginAsync();
        }

        public void Stop()
        {
            throw new System.NotImplementedException();
        }
    }
}