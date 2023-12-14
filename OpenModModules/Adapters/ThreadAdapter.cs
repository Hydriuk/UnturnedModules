using Cysharp.Threading.Tasks;
using Hydriuk.UnturnedModules.Adapters;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
using OpenMod.API.Prioritization;
using System;

namespace Hydriuk.OpenModModules.Adapters
{
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
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