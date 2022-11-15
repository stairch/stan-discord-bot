using LinqToDB;
using StanDatabase.Models;
using StanDatabase.Repositories;

namespace StanDatabase.DataAccessLayer
{
    public class DiscordCategoryRepository : IDiscordCategoryRepository
    {
        private const int CHANNEL_LIMIT_PER_CATEGORY_ON_DISCORD = 50;

        public DiscordCategory GetCategoryWithChannelCapacity()
        {
            using (var db = new DbStan())
            {
                // TODO
                List<DiscordCategory> discordCategories = db.DiscordCategory.ToList();
                //discordCategories.AddRange(db.Module.Select(m => m.DiscordCategory).ToList());
                //discordCategories = discordCategories
                //    .Where(dc => discordCategories
                //        .Count(d => d.Equals(dc)) < CHANNEL_LIMIT_PER_CATEGORY_ON_DISCORD + 1)
                //    .Distinct()
                //    .ToList();
                DiscordCategory discordCategory = discordCategories.FirstOrDefault();
                // add new category when none are available
                if (discordCategory == null)
                {
                    discordCategory = new DiscordCategory(CreateNewCategoryName(db));
                    // https://github.com/linq2db/linq2db/issues/661
                    discordCategory.DiscordCategoryId = Convert.ToInt32(db.InsertWithIdentity(discordCategory));
                }

                return discordCategory;
            }
        }

        public DiscordCategory GetCategoryById(int id)
        {
            using (var db = new DbStan())
            {
                // TODO: why does this have the correct id
                Console.WriteLine(db.DiscordCategory.Select(dc => dc.DiscordCategoryId).First());
                // but not this?
                return db.DiscordCategory.Single(dc => dc.DiscordCategoryId == id);
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
