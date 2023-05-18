#if OPENMOD
using OpenMod.API.Ioc;
#endif

namespace Hydriuk.UnturnedModules.Adapters
{
#if OPENMOD
    [Service]
#endif
    public interface ITranslationAdapter
    {
        string this[string key] { get; }

        string this[string key, object arguments] { get; }
    }
}
