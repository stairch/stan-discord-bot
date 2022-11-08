using StanDatabase.Models;

namespace StanDatabase.Repositories
{
    public interface IDiscordCategoryRepository
    {
        DiscordCategory GetCategoryWithChannelCapacity();
    }
}
