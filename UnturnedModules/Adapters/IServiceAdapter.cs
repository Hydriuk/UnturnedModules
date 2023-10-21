#if OPENMOD
using OpenMod.API.Ioc;
#elif ROCKETMOD
using Rocket.API;
#endif
using System;
using System.Threading.Tasks;

namespace Hydriuk.UnturnedModules.Adapters
{
#if OPENMOD
    [Service]
#endif
    public interface IServiceAdapter : IDisposable
    {
        Task<TService> GetServiceAsync<TService>();
    }
}