using StanDatabase.Models;

namespace StanDatabase.Repositories
{
    public interface IDiscordAccountRepository
    {
        int Insert (DiscordAccount discordAccount);

        DiscordAccount? GetAccount(int discriminaterValue, string username);

        bool DoesDiscordAccountExist(int discriminatorValue, string username);
    }
}
