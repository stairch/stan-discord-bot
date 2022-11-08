using LinqToDB.Mapping;

namespace StanDatabase.Models
{
    [Table(Name = "Houses")]
    public class House
    {
        [PrimaryKey, Identity]
        public int HouseId { get; set; }

        [Column(Name = $"{nameof(House)}{nameof(Name)}")]
        public string Name { get; set; }

        [Column, NotNull]
        public int FkDiscordRoleId { get; set; }

        [Association(ThisKey = nameof(FkDiscordRoleId), OtherKey = nameof(Models.DiscordRole.DiscordRoleId))]
        public DiscordRole DiscordRole { get; set; }
    }
}
