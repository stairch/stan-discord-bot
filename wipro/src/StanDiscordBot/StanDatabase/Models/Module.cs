using LinqToDB.Mapping;

namespace StanDatabase.Models
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
        public int FkDiscordCategoryId { get; set; }

        [Association(ThisKey = nameof(FkDiscordCategoryId), OtherKey = nameof(Models.DiscordCategory.DiscordCategoryId))]
        public DiscordCategory DiscordCategory { get; set; }

        public override string ToString()
        {
            return $"{base.ToString()}[" +
                $"{nameof(ModuleId)}: {ModuleId}, " +
                $"{nameof(ChannelName)}: {ChannelName}, " +
                $"{nameof(FullModuleName)}: {FullModuleName}, " +
                $"{nameof(FkDiscordCategoryId)}: {FkDiscordCategoryId}" +
                $"]";
        }
    }
}
