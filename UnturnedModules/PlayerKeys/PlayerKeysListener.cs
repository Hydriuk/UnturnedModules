using SDG.Unturned;
using System;
using UnityEngine;

namespace Hydriuk.UnturnedModules.PlayerKeys
{
    public delegate void KeyStateChanged(Player player, EPlayerKey key, bool state);

    public class PlayerKeysListener : MonoBehaviour, IDisposable
    {
        public static event KeyStateChanged? KeyStateChanged;

        private PlayerInput? _input;

        private bool[] _keyStates = new bool[0];

        private void Awake()
        {
            _input = GetComponentInParent<PlayerInput>();
            _keyStates = new bool[_input.keys.Length];
        }

        public void Dispose()
        {
            Destroy(this);
        }

        private void FixedUpdate()
        {
            if (_input == null)
                throw new NullReferenceException("PlayerKeysListener was attached to a non player object");

            for (int key = 0; key < _keyStates.Length; key++)
            {
                if (_keyStates[key] != _input.keys[key])
                {
                    _keyStates[key] = _input.keys[key];
                    KeyStateChanged?.Invoke(_input.player, (EPlayerKey)key, _input.keys[key]);
                }
            }
        }
    }
}