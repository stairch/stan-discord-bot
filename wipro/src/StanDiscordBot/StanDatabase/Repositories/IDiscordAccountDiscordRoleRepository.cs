using StanDatabase.Models;

namespace StanDatabase.Repositories
{
    public interface IDiscordAccountDiscordRoleRepository
    {
        public int Insert(DiscordAccountDiscordRole discordAccountDiscordRole);

        List<DiscordRole> GetRolesForAccount(int discordAccountId);

        void ChangeStillStudyingRole(int discordAccountId, int oldRoleId, DiscordRole newRole);

        DiscordRole? GetHouseRoleForAccount(int discordAccountId);

        void Delete(int discordAccountId, int discordRoleId);
    }
}
