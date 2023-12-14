using SDG.Unturned;

namespace Hydriuk.UnturnedModules.Adapters
{
    public interface ICommandAdapter
    {
        void Execute(Player player, string command);
        void Execute(string command);
    }
}