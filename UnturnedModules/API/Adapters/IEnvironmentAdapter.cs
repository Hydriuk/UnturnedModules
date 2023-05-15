#if OPENMOD
using OpenMod.API.Ioc;
#endif

namespace Hydriuk.UnturnedModules.API.Adapters
{
#if OPENMOD
    [Service]
#endif
    public interface IEnvironmentAdapter
    {
        string Directory { get; }
    }
}
