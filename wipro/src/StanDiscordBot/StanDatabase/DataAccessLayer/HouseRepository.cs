using LinqToDB;
using StanDatabase.Models;
using StanDatabase.Repositories;

namespace StanDatabase.DataAccessLayer
{
    public class HouseRepository : IHouseRepository
    {
        public House GetHouseByName(string houseName)
        {
            using (var db = new DbStan())
            {
                return db.House.Single(h => h.Name.ToLower().Equals(houseName.ToLower()));
            }
        }

        public bool IsHouseNameValid(string houseName)
        {
            using (var db = new DbStan())
            {
                return db.House.Any(h => h.Name.Equals(houseName));
            }
        }

        public DiscordRole getRoleForHouse(House house)
        {
            using(var db = new DbStan())
            {
                return db.House.LoadWith(h => h.DiscordRole).Single(h => h.HouseId == house.HouseId).DiscordRole;
            }
        }
    }
}
