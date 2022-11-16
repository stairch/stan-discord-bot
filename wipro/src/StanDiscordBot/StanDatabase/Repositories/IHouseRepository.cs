using StanDatabase.Models;

namespace StanDatabase.Repositories
{
    public interface IHouseRepository
    {
        House GetHouseByName(string houseName);

        bool IsHouseNameValid(string houseName);
    }
}
