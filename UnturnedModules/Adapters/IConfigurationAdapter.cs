#if OPENMOD
using OpenMod.API.Ioc;
#endif

namespace Hydriuk.UnturnedModules.Adapters
{
    /// <summary>
    /// Plugin configuraton wrapper
    /// </summary>
    /// <typeparam name="TConfiguration">The plugin's configuration type</typeparam>
#if OPENMOD
    [Service]
#endif
    public interface IConfigurationAdapter<TConfiguration> where TConfiguration : class, new()
    {
        /// <summary>
        /// The plugin's configuration instance
        /// </summary>
        TConfiguration Configuration { get; }
    }
}