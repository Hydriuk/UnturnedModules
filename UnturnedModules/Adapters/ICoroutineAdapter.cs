using System;

namespace Hydriuk.UnturnedModules.Adapters
{
    public interface ICoroutineAdapter : IDisposable
    {
        void RunOnFixedUpdate(Guid reference, Action action);
        void CancelFixedUpdate(Guid reference);
    }
}