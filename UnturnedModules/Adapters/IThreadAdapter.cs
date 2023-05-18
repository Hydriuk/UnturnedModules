#if OPENMOD
using OpenMod.API.Ioc;
#endif
using System;

namespace Hydriuk.UnturnedModules.Adapters
{
#if OPENMOD
    [Service]
#endif
    public interface IThreadAdapter
    {
        void RunOnMainThread(Action action);
        void RunOnThreadPool(Action action);
    }
}