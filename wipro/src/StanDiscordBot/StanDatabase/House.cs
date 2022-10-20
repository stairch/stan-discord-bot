using LinqToDB.Mapping;

namespace StanDatabase
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

        [Association(ThisKey = nameof(FkDiscordRoleId), OtherKey = nameof(StanDatabase.DiscordRole.DiscordRoleId))]
        public DiscordRole DiscordRole { get; set; }

        public static int GetHouseIdByName(string houseName)
        {
            // TODO
            return 1;
        }

        public static House GetHouseById(int houseId)
        {
            // TODO
            return new House();
        }

        public static House GetHouseByName(string houseName)
        {
            return GetHouseById(GetHouseIdByName(houseName));
        }

        public static bool IsHouseNameValid(string houseName)
        {
            // TODO: check in DB
            return true;
        }
    }
}
