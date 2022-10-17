using LinqToDB.Mapping;
using System.Numerics;

namespace StanDatabase
{
    [Table(Name = "Modules")]
    public class Module
    {
        [PrimaryKey, Identity]
        public int ModuleId { get; set; }

        /// <summary>
        /// Is the same as short module name
        /// </summary>
        [Column, NotNull]
        public string ChannelName { get; set; }

        [Column]
        public string FullModuleName { get; set; }

        [Column]
        public int FkCategoryId { get; set; }

        [Association(ThisKey = nameof(FkCategoryId), OtherKey = nameof(StanDatabase.DiscordCategory.CategoryId))]
        public DiscordCategory DiscordCategory { get; set; }
    }
}
