#if OPENMOD
using OpenMod.API.Ioc;
#endif
using System;

namespace Hydriuk.UnturnedModules.API.Adapters
{
#if OPENMOD
    [Service]
#endif
    public interface IThreadAdatper
    {
        void RunOnMainThread(Action action);
        void RunOnThreadPool(Action action);
    }
}