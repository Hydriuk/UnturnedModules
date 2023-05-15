using SDG.NetTransport;
using SDG.Unturned;
using Steamworks;

namespace Hydriuk.UnturnedModules.Extensions
{
    public static class PlayerExtensions
    {
        public static CSteamID GetSteamID(this Player player)
        {
            return player.channel.owner.playerID.steamID;
        }

        public static ITransportConnection GetTransportConnection(this Player player)
        {
            return player.channel.GetOwnerTransportConnection();
        }
    }
}