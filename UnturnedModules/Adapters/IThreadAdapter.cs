using System;

namespace Hydriuk.UnturnedModules.Adapters
{
    public interface IThreadAdapter
    {
        void RunOnMainThread(Action action);
        void RunOnThreadPool(Action action);
    }
}