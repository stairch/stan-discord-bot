using LinqToDB.Mapping;

namespace StanDatabase
{
    [Table(Name = "DiscordRoles")]
    public class DiscordRole
    {
        [PrimaryKey, Identity]
        public int DiscordRoleId { get; set; }

        [Column, NotNull]
        public string DiscordRoleName { get; set; }
    }
}
