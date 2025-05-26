using System;
using System.Linq;
using System.Text;
using CommandSystem;
using CoreLib.Other.CoreCommand;
using Exiled.API.Features;

namespace CoreLib.Commands.RemoteAdmin
{
    [CommandHandler(typeof(CustomItem))]
    public class CustomItemsList : AutoPermCheckCommand
    {
        protected override bool ExecutePermitted(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            bool isDebug = !Player.TryGet(sender, out Player player) || CustomItem.Debugs.Contains(player.UserId);

            string resp = string.Empty;
            
            foreach (CustomItems.API.Features.CustomItem item in CustomItems.API.Features.CustomItem.Registered.OrderBy(x => x.Id))
            {
                switch (item.Id)
                {
                    case < 0 when isDebug:
                        resp += $"<color=#{item.Color}>{item.Name}</color> <b>[{item.Id}] <color=#FF0000>[DEBUG]</color></b>\n";
                        break;
                    case > 0:
                        resp += $"<color=#{item.Color}>{item.Name}</color> <b>[{item.Id}]</b>\n";
                        break;
                }
            }

            response = $"\n{resp.TrimEnd('\n')}";
            return true;
        }

        public override string Command => "list";
        public override string[] Aliases => new[] {"l"};
        public override string Description => "Выводит список кастомных предметов";
    }
}