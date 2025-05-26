using System;
using CommandSystem;
using Exiled.Permissions.Extensions;

namespace CoreLib.Other.CoreCommand
{
    public abstract class AutoPermCheckCommand : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (sender.CheckPermission($"corelib.{Command}") || sender.CheckPermission("corelib.*")) return ExecutePermitted(arguments, sender, out response);
        
            response = "Недостаточно прав!";
            return false;
        }

        protected abstract bool ExecutePermitted(ArraySegment<string> arguments, ICommandSender sender, out string response);

        public abstract string Command { get; }
        public abstract string[] Aliases { get; }
        public abstract string Description { get; }
    }
}