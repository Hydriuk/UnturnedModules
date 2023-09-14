#if OPENMOD
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
#endif
using SDG.Unturned;
using System.Linq;

namespace Hydriuk.UnturnedModules.PlayerKeys
{
#if OPENMOD
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
#endif
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
                    ?.Dispose();
            }
        }

        private void AddListener(Player player)
        {
            player.gameObject.AddComponent<PlayerKeysListener>();
        }
    }
}