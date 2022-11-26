using StanDatabase.Models;

namespace StanDatabase.Repositories
{
    public interface IDiscordAccountDiscordRoleRepository
    {
        public int Insert(DiscordAccountDiscordRole discordAccountDiscordRole);

        List<string> getRolesForAccount(int discordAccountId);
    }
}
