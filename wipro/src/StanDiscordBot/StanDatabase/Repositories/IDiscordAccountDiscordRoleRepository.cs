namespace StanDatabase.Repositories
{
    public interface IDiscordAccountDiscordRoleRepository
    {
        List<string> getRolesForAccount(int discordAccountId);
    }
}
