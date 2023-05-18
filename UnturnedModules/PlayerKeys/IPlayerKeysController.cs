#if OPENMOD
using OpenMod.API.Ioc;
#endif
using System;
using System.Collections.Generic;
using System.Text;

namespace Hydriuk.UnturnedModules.PlayerKeys
{
#if OPENMOD
    [Service]
#endif
    public interface IPlayerKeysController : IDisposable
    {
    }
}
