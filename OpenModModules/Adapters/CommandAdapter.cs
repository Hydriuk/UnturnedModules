using Hydriuk.UnturnedModules.Adapters;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Commands;
using OpenMod.API.Ioc;
using OpenMod.API.Users;
using OpenMod.Core.Console;
using OpenMod.Core.Helpers;
using OpenMod.Core.Users;
using SDG.Unturned;

namespace Hydriuk.OpenModModules.Adapters
{
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Transient)]
    public class CommandAdapter : ICommandAdapter
    {
        private readonly ICommandExecutor _commandExecutor;
        private readonly IConsoleActorAccessor _consoleActorAccessor;
        private readonly IUserManager _userManager;

        public CommandAdapter(ICommandExecutor commandExecutor, IConsoleActorAccessor consoleActorAccessor, IUserManager userManager)
        {
            _commandExecutor = commandExecutor;
            _consoleActorAccessor = consoleActorAccessor;
            _userManager = userManager;
        }

        public async void Execute(Player player, string command)
        {
            IUser? user = await _userManager.FindUserAsync(KnownActorTypes.Player, player.channel.owner.playerID.steamID.ToString(), UserSearchMode.FindById);

            if (user == null)
                return;

            string[] args = ArgumentsParser.ParseArguments(command);

            await _commandExecutor.ExecuteAsync(user, args, "/");
        }

        public async void Execute(string command)
        {
            string[] args = ArgumentsParser.ParseArguments(command);

            await _commandExecutor.ExecuteAsync(_consoleActorAccessor.Actor, args, "/");
        }
    }
}