using Cysharp.Threading.Tasks;
using Hydriuk.UnturnedModules.Adapters;
using System;

namespace Hydriuk.OpenModModules.Adapters
{
    internal class ThreadAdapter : IThreadAdapter
    {
        public async void RunOnMainThread(Action action)
        {
            await UniTask.SwitchToMainThread();

            action();
        }

        public void RunOnThreadPool(Action action) => UniTask.RunOnThreadPool(action);
    }
}