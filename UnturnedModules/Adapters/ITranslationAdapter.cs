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
        Task<ITranslations> GetTranslations<T>() where T : IAdaptablePlugin;
    }

    public interface ITranslations
    {
        string this[string key] { get; }
        string this[string key, object arguments] { get; }
    }
}