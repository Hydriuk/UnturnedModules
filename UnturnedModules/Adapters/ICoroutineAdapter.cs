#if OPENMOD
using OpenMod.API.Ioc;
#endif
using System;
using System.Collections.Generic;
using System.Text;

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
