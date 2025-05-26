using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using UnityEngine;

namespace CoreLib.DevTools.Extensions
{
    public static class UnityExtensions
    {
        public static bool TryGetPlayer(this RaycastHit hit, out Player? player)
        {
            player = hit.GetPlayer();
            return player != null;
        }
    
        public static Player? GetPlayer(this RaycastHit hit)
        {
            return hit.collider.GetPlayer();
        }
    
        public static Player? GetPlayer(this Collider collider)
        {
            if (collider != null)
            {
                return Player.Get(collider);
            }
            return null;
        }
    
        public static IEnumerable<Player> GetPlayers(this IEnumerable<RaycastHit> hits)
        {
            return (from h in hits
                select h.collider).GetPlayers();
        }
    
        public static IEnumerable<Player> GetPlayers(this IEnumerable<Collider> colliders)
        {
            return (from c in colliders
                select c.GetPlayer()).ExceptNull().Distinct();
        }
    
        public static ValueTuple<float, float, float> ToHSV(this Color color)
        {
            Color.RGBToHSV(color, out var h, out var s, out var v);
            return new ValueTuple<float, float, float>(h, s, v);
        }
    
        private static IEnumerable<Player> ExceptNull(this IEnumerable<Player> enumerable)
        {
            return from p in enumerable
                where (p != null) ? p.GameObject : null
                select p;
        }
    }
}