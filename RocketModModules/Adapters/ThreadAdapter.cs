using Hydriuk.UnturnedModules.Adapters;
using Rocket.Core.Utils;
using System;

namespace Hydriuk.RocketModModules.Adapters
{
    internal class ThreadAdapter : IThreadAdapter
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