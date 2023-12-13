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
        Task<IConfProxy<TConfiguration>> GetConfiguration<TPlugin, TConfiguration>()
            where TPlugin : IAdaptablePlugin
            where TConfiguration : new();
    }

    public interface IConfProxy<T>
    {
        T Configuration { get; }
    }
}