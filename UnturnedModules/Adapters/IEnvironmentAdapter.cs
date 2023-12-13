#if OPENMOD
using OpenMod.API.Ioc;
#endif
using System.Threading.Tasks;

namespace Hydriuk.UnturnedModules.Adapters
{
#if OPENMOD
    [Service]
#endif
    public interface IEnvironmentAdapter
    {
        Task<string> GetDirectory<T>() where T : IAdaptablePlugin;
    }
}