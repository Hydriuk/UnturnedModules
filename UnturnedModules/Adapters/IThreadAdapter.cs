using System;

namespace Hydriuk.UnturnedModules.Adapters
{
    public interface IThreadAdapter
    {
        /// <summary>
        /// Runs an action on the main thread
        /// </summary>
        /// <param name="action">The action to run</param>
        void RunOnMainThread(Action action);

        /// <summary>
        /// Run an action on the thread pool
        /// </summary>
        /// <param name="action">The action to run</param>
        void RunOnThreadPool(Action action);
    }
}