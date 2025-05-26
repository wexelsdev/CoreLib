using Exiled.API.Features;
using System.Collections.Generic;

namespace CoreLib.AntiAFK.API
{
    public static class Features
    {
        public static Dictionary<Player, uint> AfkTime { get; internal set; } = new();

        public static IEnumerator<float> ReplaceAfkPlayer(this Player oldPlayer, Player newPlayer)
        {
            /*if (oldPlayer.GetCustomRole(out CustomRole? oldRole) && oldRole != null)
            {
                oldRole.RemoveRole(oldPlayer);
            }
            else
            {
                newPlayer.Role.Set(RoleTypeId.Tutorial, SpawnReason.Revived, RoleSpawnFlags.All);
                newPlayer.Role.Set(oldPlayer.Role.Type, SpawnReason.Revived, RoleSpawnFlags.None);
            }
            yield return Timing.WaitForSeconds(1);

            newPlayer.Broadcast(10, CorePlugin.Instance.Translation.MessageNewPlayer, Broadcast.BroadcastFlags.Normal, true);

            Item currentItem = oldPlayer.CurrentItem;

            newPlayer.Teleport(oldPlayer);
            newPlayer.Health = oldPlayer.Health;
            newPlayer.MaxHealth = oldPlayer.MaxHealth;
            newPlayer.HumeShield = oldPlayer.HumeShield;
            newPlayer.ArtificialHealth = oldPlayer.ArtificialHealth;
            newPlayer.Rotation = oldPlayer.Rotation;
            newPlayer.Stamina = oldPlayer.Stamina;
            newPlayer.MaxArtificialHealth = oldPlayer.MaxArtificialHealth;
            newPlayer.IsSpawnProtected = oldPlayer.IsSpawnProtected;
            
            if (oldPlayer.IsCuffed) newPlayer.Handcuff(oldPlayer.Cuffer);
            
            foreach (var effect in oldPlayer.ActiveEffects) newPlayer.EnableEffect(effect.GetEffectType(), effect.Duration);
            foreach (Item item in oldPlayer.Items) item.ChangeItemOwner(oldPlayer, newPlayer);
            
            if (currentItem != null) newPlayer.CurrentItem = currentItem;

            oldPlayer.Role.Set(RoleTypeId.Spectator, SpawnReason.ForceClass, RoleSpawnFlags.All);
            */
            
            yield break;
        }
    }
}
