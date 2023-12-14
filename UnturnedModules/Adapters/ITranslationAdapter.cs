namespace Hydriuk.UnturnedModules.Adapters
{
    public interface ITranslationAdapter 
    {
        string this[string key] { get; }
        string this[string key, object arguments] { get; }
    }
}