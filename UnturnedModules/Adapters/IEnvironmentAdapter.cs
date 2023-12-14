namespace Hydriuk.UnturnedModules.Adapters
{
    public interface IEnvironmentAdapter<TPlugin> where TPlugin : IAdaptablePlugin
    {
        string Directory { get; }
    }
}