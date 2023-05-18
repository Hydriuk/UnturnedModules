using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UIElements;

namespace Hydriuk.UnturnedModules.PlayerKeys
{
    public class PlayerKeysController : IPlayerKeysController
    {
        public PlayerKeysController() 
        {
            Player.onPlayerCreated += AddListener;

            foreach (var player in Provider.clients
                .Select(sPlayer => sPlayer.player))
            {
                AddListener(player);
            }
        }

        public void Dispose()
        {
            Player.onPlayerCreated -= AddListener;

            foreach (var player in Provider.clients
                .Select(sPlayer => sPlayer.player))
            {
                player.gameObject
                    .GetComponent<PlayerKeysListener>()
                    .Dispose();
            }
        }

        private void AddListener(Player player)
        {
            player.gameObject.AddComponent<PlayerKeysListener>();
        }
    }
}
