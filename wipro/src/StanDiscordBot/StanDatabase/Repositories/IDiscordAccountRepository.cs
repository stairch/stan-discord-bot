using StanDatabase.Models;

namespace StanDatabase.Repositories
{
    public interface IDiscordAccountRepository
    {
        int Insert (DiscordAccount discordAccount);

        int Update (DiscordAccount discordAccount);

        int Delete (int discordAccountId);
    }
}
