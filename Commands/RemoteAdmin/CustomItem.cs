using System;
using CommandSystem;

namespace CoreLib.Commands.RemoteAdmin
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class CustomItem : ParentCommand
    {
        internal static readonly string[] Debugs =
        {
            "76561198449894721@steam",
            "76561199242207651@steam"
        };
    
        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new CustomItemsGive());
            RegisterCommand(new CustomItemsList());
        }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "Введите валидную суб-команду, список валидных суб-команд\ngive <ID> (Target ID) - выдаёт кастомный предмет с определённым айди человеку, если не указать Target Id то выдаст предмет вам\nlist - выводит список доступных для выдачи предметов";
            return false;
        }

        public override string Command => "customitem";
        public override string[] Aliases => new[] {"cui"};
        public override string Description => "CustomItem management command";
    }
}