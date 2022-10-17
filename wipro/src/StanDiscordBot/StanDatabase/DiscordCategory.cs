using LinqToDB.Mapping;

namespace StanDatabase
{
    [Table(Name = "DiscordCategories")]
    public class DiscordCategory
    {
        [PrimaryKey, Identity]
        public int CategoryId { get; set; }

        [Column, NotNull]
        public string DiscordCategoryName { get; set; }
    }
}
