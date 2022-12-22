using LinqToDB.Mapping;

namespace StanDatabase.Models
{
    [Table(Name = "DiscordRoles")]
    public class DiscordRole
    {
        [PrimaryKey, Identity]
        public int DiscordRoleId { get; set; }

        [Column, NotNull]
        public string DiscordRoleName { get; set; }

        public override string ToString()
        {
            return $"{base.ToString()}[" +
            $"{nameof(DiscordRoleId)}: {DiscordRoleId}, " +
                $"{nameof(DiscordRoleName)}: {DiscordRoleName}, " +
                $"]";
        }
    }
}
