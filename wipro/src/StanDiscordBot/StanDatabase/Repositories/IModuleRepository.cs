using StanDatabase.Models;

namespace StanDatabase.Repositories
{
    public interface IModuleRepository
    {
        void InsertMultiple(IList<Module> modules);

        void RemoveOld(IList<Module> modules);

        bool DoesModuleExist(string moduleName);

        Module GetModuleByName(string moduleName);
    }
}
