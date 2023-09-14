#if OPENMOD
using OpenMod.API.Ioc;
#endif
using System;

namespace Hydriuk.UnturnedModules.Adapters
{
#if OPENMOD
    [Service]
#endif
    public interface ICoroutineAdapter : IDisposable
    {
        void RunOnFixedUpdate(Guid reference, Action action);
        void CancelFixedUpdate(Guid reference);
    }
}