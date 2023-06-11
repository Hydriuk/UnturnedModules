using Hydriuk.UnturnedModules.Adapters;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hydriuk.RocketModModules.Adapters
{
    public class CoroutineAdapter : MonoBehaviour, ICoroutineAdapter
    {
        private readonly Dictionary<Guid, Action> _fixedUpdateActions = new Dictionary<Guid, Action>();

        private bool _isRunning = false;

        public void Dispose()
        {
            _fixedUpdateActions.Clear();
            Destroy(this);
        }

        IEnumerator RunUpdate()
        {
            if (!_isRunning)
            {
                _isRunning = true;

                while (_fixedUpdateActions.Count > 0)
                {
                    foreach (Action action in _fixedUpdateActions.Values)
                    {
                        action();
                    }

                    yield return new WaitForFixedUpdate();
                }

                _isRunning = false;
            }
        }

        public void RunOnFixedUpdate(Guid reference, Action action)
        {
            _fixedUpdateActions.Add(reference, action);

            StartCoroutine(RunUpdate());
        }

        public void CancelFixedUpdate(Guid reference)
        {
            _fixedUpdateActions.Remove(reference);
        }
    }
}
