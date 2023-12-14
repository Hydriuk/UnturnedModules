#if OPENMOD
using OpenMod.API.Ioc;
#endif
using System.Threading.Tasks;

namespace Hydriuk.UnturnedModules.Adapters
{
#if OPENMOD
    [Service]
#endif
    public interface ITranslationAdapter 
    {
        ITranslations GetTranslations();
    }

    public interface ITranslations
    {
        string this[string key] { get; }
        string this[string key, object arguments] { get; }
    }
}