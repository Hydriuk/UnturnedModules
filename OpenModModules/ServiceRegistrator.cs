using Hydriuk.OpenModModules.Adapters;
using Hydriuk.UnturnedModules.Adapters;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
using OpenMod.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hydriuk.OpenModModules
{
    public class ServiceRegistrator
    {
        public static void ConfigureServices<TPlugin, TConfiguration>(IOpenModServiceConfigurationContext openModStartupContext, IServiceCollection serviceCollection) 
            where TConfiguration : class, new()
        {
            serviceCollection.AddSingleton<IServiceAdapter, ServiceAdapter>();
            serviceCollection.AddSingleton<IThreadAdapter, ThreadAdapter>();
            serviceCollection.AddSingleton<ICommandAdapter, CommandAdapter>();
            serviceCollection.AddSingleton<IPermissionAdapter, PermissionAdapter>();
            serviceCollection.AddSingleton<ITranslationAdapter, TranslationAdapter>();
            serviceCollection.AddSingleton<IEnvironmentAdapter, EnvironmentAdapter<TPlugin>>();
            serviceCollection.AddSingleton<IConfigurationAdapter<TConfiguration>, ConfigurationAdapter<TConfiguration>>();
        }

        public static void ConfigureServices<TPlugin>(IOpenModServiceConfigurationContext openModStartupContext, IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IServiceAdapter, ServiceAdapter>();
            serviceCollection.AddSingleton<IThreadAdapter, ThreadAdapter>();
            serviceCollection.AddSingleton<ICommandAdapter, CommandAdapter>();
            serviceCollection.AddSingleton<IPermissionAdapter, PermissionAdapter>();
            serviceCollection.AddSingleton<IEnvironmentAdapter, EnvironmentAdapter<TPlugin>>();
            serviceCollection.AddSingleton<ITranslationAdapter, TranslationAdapter>();
        }
    }
}
