using StanDatabase.DTOs;
using StanDatabase.Models;

namespace StanDatabase.Repositories
{
    public interface IDiscordAccountModuleRepository
    {
        /// <summary>
        /// Returns the number of members per module.
        /// Sorted by members count and with a configurable limit.
        /// </summary>
        /// <param name="limit">Top <paramref name="limit"/> modules will be returned</param>
        /// <returns>List of MembersPerModuleDTO.</returns>
        List<MembersPerModuleDTO> NumberOfMembersPerModule(int limit = 10);

        public int Insert(DiscordAccountModule discordAccountModule);
    }
}
