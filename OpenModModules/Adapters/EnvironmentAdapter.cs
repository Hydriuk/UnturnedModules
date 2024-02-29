using Hydriuk.UnturnedModules.Adapters;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API;
using OpenMod.API.Plugins;
using OpenMod.Core.Plugins;
using OpenMod.Runtime;
using System;
using System.Linq;
using System.Reflection;

namespace Hydriuk.OpenModModules.Adapters
{
    public class EnvironmentAdapter<TPlugin> : IEnvironmentAdapter
        where TPlugin : IOpenModComponent
    {
        public string Directory { get; private set; }

        public EnvironmentAdapter(IServiceProvider serviceProvider)
        {
            IRuntime runtime = serviceProvider.GetRequiredService<IRuntime>();
            PluginMetadataAttribute metadata = typeof(TPlugin).Assembly.GetCustomAttribute<PluginMetadataAttribute>();

            Directory = PluginHelper.GetWorkingDirectory(runtime, metadata.Id);
        }
    }
}