using Hydriuk.RocketModModules.Adapters;
using Hydriuk.UnturnedModules.Adapters;
using Rocket.Core.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RocketModModules
{
    public class RocketModModules : RocketPlugin
    {
        public static IServiceAdapter ServiceAdapter { get; private set; }
        public static IThreadAdapter ThreadAdapter { get; private set; }
        public static ICommandAdapter CommandAdapter { get; private set; }
        public static IPermissionAdapter PermissionAdapter { get; private set; }
        public static IEnvironmentAdapter EnvironmentAdapter { get; private set; }
        public static ITranslationAdapter TranslationAdapter { get; private set; }
        public static IConfigurationAdapter ConfigurationAdapter { get; private set; }

        protected override void Load()
        {
            ServiceAdapter = new ServiceAdapter();

            ThreadAdapter = new ThreadAdapter();
            CommandAdapter = new CommandAdapter();
            PermissionAdapter = new PermissionAdapter();
            EnvironmentAdapter = new EnvironmentAdapter(ServiceAdapter as ServiceAdapter);
            TranslationAdapter = new TranslationAdapter(ServiceAdapter as ServiceAdapter);
            ConfigurationAdapter = new ConfigurationAdapter(ServiceAdapter as ServiceAdapter);
        }

        protected override void Unload()
        {
            ServiceAdapter?.Dispose();
        }
    }
}
