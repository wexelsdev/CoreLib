using System;
using System.Linq;
using CommandSystem;
using CoreLib.Other.CoreCommand;
using Exiled.API.Features;

namespace CoreLib.Commands.RemoteAdmin
{
    [CommandHandler(typeof(CustomRole))]
    public class CustomRolesGive : AutoPermCheckCommand, IUsageProvider
    {
        protected override bool ExecutePermitted(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!Player.TryGet(sender, out Player player))
            {
                response = "Бля пиздуй нахуй с консоли тварь";
                return false;
            }

            if (arguments.Count < 1)
            {
                response = $"Неправильное использование команды, юзай: {Usage.First()}";
                return false;
            }

            switch (arguments.Count)
            {
                case 1:
                    if (!int.TryParse(arguments.Array![2], out int id))
                    {
                        response = "Введи число пжпжпж";
                        return false;
                    }

                    if (CustomRoles.API.Features.CustomRole.Registered.FirstOrDefault(x => x.Id == id) == null)
                    {
                        response = "Броу, с тобой всё нормально? Такой роли нету, напиши customrole list";
                        return false;
                    }

                    CustomRoles.API.Features.CustomRole.Registered.First(x => x.Id == id).AddRole(player, true, true);
                    response = "Успешно бля";
                    return true;
            
                case > 1:
                    if (!int.TryParse(arguments.Array![2], out id))
                    {
                        response = "Введи число пжпжпж";
                        return false;
                    }
                
                    if (!int.TryParse(arguments.Array[3], out int targetId))
                    {
                        response = "Введи число пжпжпж";
                        return false;
                    }

                    if (!Player.TryGet(targetId, out player))
                    {
                        response = $"Игрока с айди {targetId} не существует";
                        return false;
                    }
                
                    if (CustomRoles.API.Features.CustomRole.Registered.FirstOrDefault(x => x.Id == id) == null)
                    {
                        response = "Броу, с тобой всё нормально? Такой роли нету, напиши customrole list";
                        return false;
                    }

                    CustomRoles.API.Features.CustomRole.Registered.First(x => x.Id == id).AddRole(player, true, true);
                    response = "Успешно бля";
                    return true;
            
                default:
                    response = "Вам отказано, причина иди нахуй";
                    return false;
            }
        }

        public override string Command => "give";
        public override string[] Aliases => new[] { "g" };
        public override string Description => "Выдаёт вам кастомную роль";
        public string[] Usage => new[] {$"{Command} <ID> (Target ID)"};
    }
}