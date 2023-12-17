using SDG.Unturned;

namespace Hydriuk.UnturnedModules.Adapters
{
    /// <summary>
    /// Plugin command wrapper
    /// </summary>
    public interface ICommandAdapter
    {
        /// <summary>
        /// Execute a command in place of a player
        /// </summary>
        /// <param name="player">The player to execute the command for</param>
        /// <param name="command">The command to execute</param>
        void Execute(Player player, string command);

        /// <summary>
        /// Execute a command as console
        /// </summary>
        /// <param name="command">The command to execute</param>
        void Execute(string command);
    }
}