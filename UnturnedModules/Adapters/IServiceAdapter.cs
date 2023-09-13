#if OPENMOD
using OpenMod.API.Ioc;
#elif ROCKETMOD
using Rocket.API;
#endif
using System.Threading.Tasks;

namespace Hydriuk.UnturnedModules.Adapters
{
#if OPENMOD
    [Service]
#endif
    public interface IServiceAdapter 
    {
        Task<TService> GetServiceAsync<TService>();
    }
}
