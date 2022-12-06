using StanDatabase.Models;

namespace StanDatabase.Repositories
{
    public interface IDiscordAccountRepository
    {
        int Insert (DiscordAccount discordAccount);

        int Update (DiscordAccount discordAccount);

        int Delete (int discordAccountId);

        bool IsAdmin(string username);

        bool IsStillStudying(string username);

        bool IsExstudent(string username);

        House GetHouseFromStudent(string username);

        DiscordAccount GetDiscordAccountByName(string username);
    }
}
