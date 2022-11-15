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
                // TODO: rewrite this.
                // this can't be done since modules aren't added in the meantime
                List<DiscordCategory> discordCategories = db.Module.Select(m => m.DiscordCategory).ToList();
                discordCategories.AddRange(db.DiscordCategory);
                discordCategories = discordCategories
                    .Where(dc => discordCategories
                        .Count(d => d.Equals(dc)) < CHANNEL_LIMIT_PER_CATEGORY_ON_DISCORD + 1)
                    .Distinct()
                    .ToList();
                DiscordCategory discordCategory = discordCategories.FirstOrDefault();
                // add new category when none are available
                if (discordCategory == null)
                {
                    discordCategory = new DiscordCategory(CreateNewCategoryName());
                    db.Insert(discordCategory);
                    discordCategory = db.DiscordCategory.Single(dc => dc.DiscordCategoryName.Equals(discordCategory.DiscordCategoryName));
                }

                // TODO: check why this is necessary (see comment below)
                if (discordCategory.DiscordCategoryId == 0)
                {
                    discordCategory.DiscordCategoryId = db.DiscordCategory.First(dc => dc.DiscordCategoryName.Equals(discordCategory.DiscordCategoryName)).DiscordCategoryId;
                    discordCategory = GetCategoryById(1);
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

        private string CreateNewCategoryName()
        {
            using (var db = new DbStan())
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
}
