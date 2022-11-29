namespace StanDatabase.Repositories
{
    public interface IDiscordAccountModuleRepository
    {
        void AddModuleToUser(string user, string moduleName);
    }
}
