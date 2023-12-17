namespace Hydriuk.UnturnedModules.Adapters
{
    /// <summary>
    /// This service gives access to some plugin data
    /// </summary>
    public interface IEnvironmentAdapter
    {
        /// <summary>
        /// Directory of the plugin
        /// </summary>
        string Directory { get; }
    }
}