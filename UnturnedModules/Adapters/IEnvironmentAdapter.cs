namespace Hydriuk.UnturnedModules.Adapters
{
    /// <summary>
    /// This service gives access to some plugin data
    /// Do not call this service if the plugin was not loaded yet
    /// </summary>
    public interface IEnvironmentAdapter
    {
        string Directory { get; }
    }
}