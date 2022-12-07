using StanDatabase.Models;

namespace StanDatabase.Repositories
{
    public interface IDiscordRoleRepository
    {
        DiscordRole GetRoleByName(string roleName);
    }
}