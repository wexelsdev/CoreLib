using System;
using System.Linq;
using System.Text;
using CommandSystem;
using CoreLib.Other.CoreCommand;
using Exiled.API.Features;

namespace CoreLib.Commands.RemoteAdmin
{
    [CommandHandler(typeof(CustomRole))]
    public class CustomRolesList : AutoPermCheckCommand
    {
        protected override bool ExecutePermitted(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            bool isDebug = !Player.TryGet(sender, out Player player) || CustomRole.Debugs.Contains(player.UserId);

            string resp = string.Empty;
            
            foreach (CustomRoles.API.Features.CustomRole role in CustomRoles.API.Features.CustomRole.Registered.OrderBy(x => x.Id))
            {
                switch (role.Id)
                {
                    case < 0 when isDebug:
                        resp += $"{role.Name} <b>[{role.Id}] <color=#FF0000>[DEBUG]</color></b>\n";
                        break;
                    case > 0:
                        resp += $"{role.Name} <b>[{role.Id}]</b>\n";
                        break;
                }
            }

            response = $"\n{resp.TrimEnd('\n')}";
            return true;
        }

        public override string Command => "list";
        public override string[] Aliases => new [] { "l" };
        public override string Description => "Выводит список кастомных ролей";
    }
}