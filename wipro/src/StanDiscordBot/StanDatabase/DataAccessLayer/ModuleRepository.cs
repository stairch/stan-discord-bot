using StanDatabase.Models;
using StanDatabase.Repositories;

namespace StanDatabase.DataAccessLayer
{
    public class ModuleRepository : IModuleRepository
    {
        public void InsertMultiple(IList<Module> modules)
        {
            throw new NotImplementedException();
        }

        public void RemoveOld(IList<Module> modules)
        {
            throw new NotImplementedException();
        }
    }
}
