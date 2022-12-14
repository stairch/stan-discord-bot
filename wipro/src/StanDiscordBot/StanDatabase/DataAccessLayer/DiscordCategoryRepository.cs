using LinqToDB;
using StanDatabase.Models;
using StanDatabase.Repositories;

namespace StanDatabase.DataAccessLayer
{
    public class DiscordCategoryRepository : IDiscordCategoryRepository
    {
        private const int CHANNEL_LIMIT_PER_CATEGORY_ON_DISCORD = 50;

        public bool DoesCategoryExist(string name)
        {
            using (var db = new DbStan())
            {
                return db.DiscordCategory.Any(dc => dc.DiscordCategoryName.Equals(name));
            }
        }

        public DiscordCategory GetCategoryWithChannelCapacity()
        {
            using (var db = new DbStan())
            {
                var latestCategoryQuery =   from dc in db.DiscordCategory
                                            orderby dc.DiscordCategoryId descending
                                            select dc;

                DiscordCategory? discordCategory = latestCategoryQuery.FirstOrDefault();
                // add new category when none are available
                if (discordCategory == null)
                {
                    discordCategory = new DiscordCategory();
                    discordCategory.DiscordCategoryName = CreateNewCategoryName(db);
                    discordCategory.DiscordCategoryId = Convert.ToInt32(db.InsertWithIdentity(discordCategory));
                }

                // look how many modules in this category exist
                int moduleCountInCategroy = (from m in db.Module
                                            where m.FkDiscordCategoryId == discordCategory.DiscordCategoryId
                                            select m).Count();

                // if number of modules exceed capacity limit, create a new category
                if (moduleCountInCategroy >= CHANNEL_LIMIT_PER_CATEGORY_ON_DISCORD)
                {
                    discordCategory = new DiscordCategory();
                    discordCategory.DiscordCategoryName = CreateNewCategoryName(db);
                    discordCategory.DiscordCategoryId = Convert.ToInt32(db.InsertWithIdentity(discordCategory));
                }
                Console.WriteLine(discordCategory);
                return discordCategory;
            }
        }

        private string CreateNewCategoryName(DbStan db)
        {
            string newCategoryName;
            int i = 0;
            do
            {
                i++;
                newCategoryName = $"MODULE CHANNELS {i}";
            } while (db.DiscordCategory.Any(dc => dc.DiscordCategoryName.Equals(newCategoryName)));
            return newCategoryName;
        }
    }
}
