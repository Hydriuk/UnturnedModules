using Cysharp.Threading.Tasks;
using Hydriuk.UnturnedModules.API.Adapters;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
using System;

namespace Hydriuk.OpenModModules.Adapters
{
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
    public class ThreadAdapter : IThreadAdatper
    {
        public async void RunOnMainThread(Action action)
        {
            await UniTask.SwitchToMainThread();

            action();
        }

        public void RunOnThreadPool(Action action) => UniTask.RunOnThreadPool(action);
    }
}