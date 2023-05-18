#if OPENMOD
using OpenMod.API.Ioc;
#endif

namespace Hydriuk.UnturnedModules.Adapters
{
#if OPENMOD
    [Service]
#endif
    public interface IConfigurationAdapter<T> where T : new()
    {
        T Configuration { get; }
    }
}