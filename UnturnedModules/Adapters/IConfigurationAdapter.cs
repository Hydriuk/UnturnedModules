#if OPENMOD
using OpenMod.API.Ioc;
#endif
using System.Threading.Tasks;

namespace Hydriuk.UnturnedModules.Adapters
{
#if OPENMOD
    [Service]
#endif
    public interface IConfigurationAdapter 
    {
        TConfiguration GetConfiguration<TConfiguration>()
            where TConfiguration : class, new();
    }
}