using Hydriuk.UnturnedModules.API.Adapters;
using Rocket.Core.Utils;
using System;

namespace Hydriuk.RocketModModules.Adapters
{
    public class ThreadAdapter : IThreadAdatper
    {
        public void RunOnMainThread(Action action)
        {
            TaskDispatcher.QueueOnMainThread(action);
        }

        public void RunOnThreadPool(Action action)
        {
            TaskDispatcher.RunAsync(action);
        }
    }
}