using LinqToDB;
using StanDatabase.Models;
using StanDatabase.Repositories;

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
                IList<string> currentModuleChannels = modules
                    .Select(cmc => cmc.ChannelName)
                    .ToList();

                IList<Module> oldModuleChannels = db.Module
                    .Where(omc => currentModuleChannels.Contains(omc.ChannelName))
                    .ToList();

                foreach (Module oldModule in oldModuleChannels)
                {
                    // TODO: set inactive
                    //db.Module
                    //.Where(m => m.ChannelName == oldModule.ChannelName)
                    //.Set(m => m.active, false)
                    //.Update();
                    // TODO: how to update discord role on server from here?
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
