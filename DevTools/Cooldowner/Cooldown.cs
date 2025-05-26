using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreLib.DevTools.Cooldowner
{
    public class Cooldown : IDisposable
    {
        public static HashSet<Cooldown> Cooldowns { get; } = new();
    
        public object Owner { get; private set; }
        public TimeSpan CooldownTime { get; private set; }

        private DateTime _lastUse = DateTime.UtcNow;

        public Cooldown(object owner, TimeSpan cooldownTime)
        {
            Owner = owner;
            CooldownTime = cooldownTime;
        }

        public void Use(bool overrideCooldown = false)
        {
            if (overrideCooldown)
            {
                _lastUse = DateTime.UtcNow;
                return;
            }

            if (!Check())
                return;

            _lastUse = DateTime.UtcNow;
        }

        public double GetRemaining() => (DateTime.UtcNow - (_lastUse + CooldownTime)).TotalSeconds;
    
        public bool Check() => DateTime.UtcNow > _lastUse + CooldownTime;

        public static Cooldown? Get(object owner) => Cooldowns.FirstOrDefault(x => x.Owner == owner);

        public static HashSet<Cooldown>? Get(TimeSpan cooldown) =>
            Cooldowns.Count(x => x.CooldownTime == cooldown) >= 1
                ? Cooldowns.Where(x => x.CooldownTime == cooldown).ToHashSet()
                : null;
    
        public static bool TryGet(object owner, out Cooldown? cooldown)
        {
            cooldown = Get(owner);
            return cooldown != null;
        }

        public static bool TryGet(TimeSpan cooldown, out HashSet<Cooldown>? cooldowns)
        {
            cooldowns = Get(cooldown);
            return cooldowns != null;
        }

        public void Dispose()
        {
            Cooldowns.Remove(this);
        }
    }
}