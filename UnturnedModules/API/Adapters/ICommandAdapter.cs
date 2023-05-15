#if OPENMOD
using OpenMod.API.Ioc;
#endif
using SDG.Unturned;

namespace Hydriuk.UnturnedModules.API.Adapters
{
#if OPENMOD
    [Service]
#endif
    public interface ICommandAdapter
    {
        void Execute(Player player, string command);
    }
}