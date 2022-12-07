using StanDatabase.DTOs;
using StanDatabase.Models;

namespace StanDatabase.Repositories
{
    public interface IDiscordAccountRepository
    {
        int Insert (DiscordAccount discordAccount);

        DiscordAccount? GetAccount(int discriminaterValue, string username);

        bool DoesDiscordAccountExist(int discriminatorValue, string username);

        List<DiscordAccountsPerSemesterDTO> NumberOfDiscordAccountsPerSemester();

        int Delete (int discordAccountId);

        bool IsAdmin(string username);

        bool IsStillStudying(string username);

        bool IsExstudent(string username);

        House GetHouseFromStudent(string username);

        DiscordAccount GetDiscordAccountByName(string username);
    }
}
