#if OPENMOD
using OpenMod.API.Ioc;
#endif
using SDG.Unturned;

namespace Hydriuk.UnturnedModules.Adapters
{
#if OPENMOD
    [Service]
#endif
    public interface ICommandAdapter
    {
        void Execute(Player player, string command);
        void Execute(string command);
    }
}