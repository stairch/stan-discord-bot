using LinqToDB;
using StanDatabase.Models;
using StanDatabase.Repositories;
using System.Linq;

namespace StanDatabase.DataAccessLayer
{
    public class ModuleRepository : IModuleRepository
    {
        private readonly IDiscordCategoryRepository _discordCategoryRepository;

        public ModuleRepository(IDiscordCategoryRepository discordCategoryRepository)
        {
            _discordCategoryRepository = discordCategoryRepository;
        }

        public void InsertMultiple(IList<Module> modules)
        {
            using (var db = new DbStan())
            {

                foreach (Module module in modules)
                {
                    bool moduleDoesntExistYet = !db.Module.Any(m => m.ChannelName == module.ChannelName);
                    if (moduleDoesntExistYet)
                    {
                        DiscordCategory category = _discordCategoryRepository.GetCategoryWithChannelCapacity();
                        module.FkDiscordCategoryId = category.DiscordCategoryId;
                        module.DiscordCategory = category;
                        db.Insert(module);

                    }
                    else
                    {
                        db.Module
                            .Where(m => m.ChannelName == module.ChannelName)
                            .Set(m => m.FullModuleName, module.FullModuleName)
                            .Update();
                    }
                }
            }
        }

        public void RemoveOld(IList<Module> modules)
        {
            using (var db = new DbStan())
            {
                IEnumerable<string> newModules = modules
                    .Select(m => m.ChannelName);

                IEnumerable<string> currentModules = db.Module
                    .Select(m => m.ChannelName);

                IList<string> oldModules = currentModules.Except(newModules).ToList();

                foreach (string moduleName in oldModules)
                {
                    db.Module
                        .Where(m => m.ChannelName == moduleName)
                        .Delete();
                }
            }
        }

        public bool DoesModuleExist(string moduleName)
        {
            using (var db = new DbStan())
            {
                return db.Module.SingleOrDefault(m => m.ChannelName.Equals(moduleName)) != null;
            }
        }

        public Module? GetModuleByName(string moduleName)
        {
            using (var db = new DbStan())
            {
                return db.Module
                    .LoadWith(m => m.DiscordCategory)
                    .SingleOrDefault(m => m.ChannelName.Equals(moduleName));
            }
        }
    }
}
