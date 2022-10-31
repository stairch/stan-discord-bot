using StanDatabase.Models;

namespace StanDatabase.Repositories
{
    public interface IModuleRepository
    {
        void InsertMultiple(IList<Module> modules);

        void RemoveOld(IList<Module> modules);
    }
}
