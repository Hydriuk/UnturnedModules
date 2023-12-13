using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OpenMod.API.Plugins;
using OpenMod.Unturned.Plugins;
using System;

[assembly: PluginMetadata("Hydriuk.OpenModModules", DisplayName = "Hydriuk.OpenModModules", Author = "Hydriuk")]

namespace Hydriuk.OpenModModules
{
    public class OpenModModules : OpenModUnturnedPlugin
    {
        public OpenModModules(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
