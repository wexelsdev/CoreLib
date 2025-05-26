using System;
using CommandSystem;

namespace CoreLib.Commands.RemoteAdmin
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class CustomRole : ParentCommand
    {
        internal static readonly string[] Debugs = 
        {
            "76561198449894721@steam",
            "76561199242207651@steam"
        };
    
        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new CustomRolesGive());
            RegisterCommand(new CustomRolesList());
        }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "Введите валидную суб-команду, список валидных суб-команд\ngive <ID> (Target ID) - выдаёт кастомную роль с определённым айди человеку, если не указать Target Id то выдаст роль вам\nlist - выводит список доступных для выдачи ролей";
            return false;
        }

        public override string Command => "customrole";
        public override string[] Aliases => new[] { "cur" };
        public override string Description => "CustomRole management command";
    }
}