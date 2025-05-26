using System;
using Exiled.API.Features;

namespace CoreLib.DevTools.Cooldowner
{
    public class PlayerCooldown : Cooldown
    {
        public Player? Player { get; private set; }
    
        public PlayerCooldown(object owner, TimeSpan cooldownTime) : base(owner, cooldownTime)
        {
            if (owner is not Player player)
                throw new InvalidCastException($"{owner} cannot be casted to Player");

            Player = player;
        }

        public static PlayerCooldown? Get(Player player)
        {
            Cooldown? cooldown = Get((object)player);

            return cooldown as PlayerCooldown;
        }

        public static bool TryGet(Player player, out PlayerCooldown? cooldown)
        {
            cooldown = Get(player);
            return cooldown != null;
        }
    }
}