#if OPENMOD
using OpenMod.API.Plugins;
using System;
#elif ROCKETMOD
using Rocket.API;
#endif

namespace Hydriuk.UnturnedModules.Adapters
{
    public interface IAdaptablePlugin :
#if OPENMOD
        IOpenModPlugin
#elif ROCKETMOD
        IRocketPlugin
#endif
    {
#if OPENMOD
        IServiceProvider ServiceProvider { get; }
#endif
    }
}