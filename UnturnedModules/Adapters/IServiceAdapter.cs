#if OPENMOD
using OpenMod.API.Ioc;
using OpenMod.API.Plugins;
#elif ROCKETMOD
using Rocket.API;
#endif
using System;
using System.Collections.Generic;
using System.Text;

namespace Hydriuk.UnturnedModules.Adapters
{
#if OPENMOD
    [Service]
#endif
    public interface IServiceAdapter 
    {
        TService GetService<TService>();
    }
}
