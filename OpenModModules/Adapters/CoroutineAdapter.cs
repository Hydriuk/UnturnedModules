using Cysharp.Threading.Tasks;
using Hydriuk.UnturnedModules.Adapters;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
using System;
using System.Collections.Generic;

namespace Hydriuk.OpenModModules.Adapters
{
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
    internal class CoroutineAdapter : ICoroutineAdapter
    {
        private readonly Dictionary<Guid, Action> _fixedUpdateActions = new Dictionary<Guid, Action>();

        private bool _isRunning = false;

        public void RunOnFixedUpdate(Guid reference, Action action)
        {
            _fixedUpdateActions.Add(reference, action);

            _ = RunUpdate();
        }

        public void Dispose()
        {
            _fixedUpdateActions.Clear();
        }

        public void CancelFixedUpdate(Guid reference)
        {
            _fixedUpdateActions.Remove(reference);
        }

        private async UniTask RunUpdate()
        {
            if (_isRunning)
                return;

            _isRunning = true;

            while (_fixedUpdateActions.Count > 0)
            {
                foreach (Action action in _fixedUpdateActions.Values)
                {
                    action();
                }

                await UniTask.WaitForFixedUpdate();
            }

            _isRunning = false;
        }
    }
}