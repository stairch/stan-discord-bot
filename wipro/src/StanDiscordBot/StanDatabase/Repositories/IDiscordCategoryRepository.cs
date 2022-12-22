using StanDatabase.Models;

namespace StanDatabase.Repositories
{
    public interface IDiscordCategoryRepository
    {
        bool DoesCategoryExist(string name);

        DiscordCategory GetCategoryWithChannelCapacity();
    }
}
