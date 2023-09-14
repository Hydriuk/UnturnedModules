#if OPENMOD
using OpenMod.API.Ioc;
#endif
using System;

namespace Hydriuk.UnturnedModules.PlayerKeys
{
#if OPENMOD
    [Service]
#endif
    public interface IPlayerKeysController : IDisposable
    {
    }
}