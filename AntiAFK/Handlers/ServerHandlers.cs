using Exiled.API.Features;
using MEC;
using PlayerRoles;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CoreLib.AntiAFK.Handlers
{
    public class ServerHandlers
    {
        public IEnumerator<float> OnRoundStart()
        {
            API.Features.AfkTime.Clear();
            Dictionary<Player, (Vector3, Quaternion)> pos = new Dictionary<Player, (Vector3, Quaternion)>();
            while (!Round.IsEnded)
            {
                foreach (Player player in Player.List.Where(x => x != null))
                {
                    if (player.IsDead || player.Role.Type == RoleTypeId.Scp079 || player.Role.Type == RoleTypeId.Tutorial)
                    {
                        pos.Remove(player);
                        API.Features.AfkTime.Remove(player);
                        continue;
                    }

                    if (pos.ContainsKey(player))
                    {
                        if (pos[player].Item1 == player.Position && pos[player].Item2 == player.Rotation)
                        {
                            Log.Debug($"[AntiAFK] | {player.Nickname} | {API.Features.AfkTime[player]}");
                            if (API.Features.AfkTime[player] >= CorePlugin.Instance!.Config.MessageAfkTime)
                            {
                                player.PlayGunSound(ItemType.ParticleDisruptor, 33);
                                player.Broadcast(2, CorePlugin.Instance.Translation.AfkMessage.Replace("[AFK_TIME]", API.Features.AfkTime[player].ToString()).Replace("[MAX_AFK_TIME]", CorePlugin.Instance.Config.MaxAfkTime.ToString()), Broadcast.BroadcastFlags.Normal, true);
                            }

                            if (API.Features.AfkTime[player] < CorePlugin.Instance.Config.MaxAfkTime)
                            {
                                API.Features.AfkTime[player]++;
                            }
                            else
                            {
                                if (Player.List.Any(x => x.Role.Type == RoleTypeId.Spectator))
                                {
                                    Timing.RunCoroutine(API.Features.ReplaceAfkPlayer(player, Player.List.Where(x => x.Role.Type == RoleTypeId.Spectator).ToList().RandomItem()));
                                }
                                else
                                {
                                    player.Role.Set(RoleTypeId.Spectator);
                                }
                            }
                        }
                        else
                        {
                            pos.Remove(player);
                            pos.Add(player, (player.Position, player.Rotation));
                            API.Features.AfkTime[player] = 0;
                        }
                    }
                    else
                    {
                        pos.Add(player, (player.Position, player.Rotation));
                        API.Features.AfkTime.Add(player, 0);
                    }
                }
                yield return Timing.WaitForSeconds(1);
            }
        }
    }
}
