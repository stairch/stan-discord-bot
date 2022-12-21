using StanDatabase.DTOs;
using StanDatabase.Models;

namespace StanDatabase.Repositories
{
    public interface IDiscordAccountRepository
    {
        int Insert (DiscordAccount discordAccount);

        DiscordAccount? GetAccount(long discriminaterValue, string username);

        bool DoesDiscordAccountExist(long discriminatorValue, string username);

        List<DiscordAccountsPerSemesterDTO> NumberOfDiscordAccountsPerSemester();

        int Delete(int discordAccountId);

        bool IsAdmin(string username);

        bool IsStillStudying(DiscordAccount discordAccount);

        House GetHouseFromStudent(string username);

        List<DiscordAccount> GetAllDiscordAdminAccounts();
    }
}
