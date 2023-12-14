namespace Hydriuk.UnturnedModules.Adapters
{
    public interface IConfigurationAdapter<TConfiguration> where TConfiguration : class, new()
    {
        TConfiguration Configuration { get; }
    }
}