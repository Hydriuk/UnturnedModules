using Hydriuk.UnturnedModules.API.Adapters;
using Rocket.Core;
using Rocket.Unturned.Player;
using SDG.Unturned;

namespace Hydriuk.RocketModModules.Adapters
{
    public class CommandAdapter : ICommandAdapter
    {
        public void Execute(Player player, string command)
        {
            UnturnedPlayer uPlayer = UnturnedPlayer.FromPlayer(player);

            R.Commands.Execute(uPlayer, command);
        }
    }
}