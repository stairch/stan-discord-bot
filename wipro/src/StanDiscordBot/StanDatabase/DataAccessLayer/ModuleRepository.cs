using LinqToDB;
using StanDatabase.Models;
using StanDatabase.Repositories;

namespace StanDatabase.DataAccessLayer
{
    public class ModuleRepository : IModuleRepository
    {
        public void InsertMultiple(IList<Module> modules)
        {
            using (var db = new DbStan())
            {
                foreach (Module module in modules)
                {
                    if (!db.Module.Any(m => m.ChannelName == module.ChannelName))
                    {
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
    }
}
