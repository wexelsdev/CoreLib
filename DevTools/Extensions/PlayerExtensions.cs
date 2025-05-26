using CoreLib.DevTools.Other;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using UnityEngine;

namespace CoreLib.DevTools.Extensions
{
    public static class PlayerExtensions
    {
        public static void AddOrDropItem(this Player player, Item item)
        {
            if (player.IsInventoryFull)
            {
                item.CreatePickup(player.Position, Quaternion.identity);
                return;
            }
            player.AddItem(item);
        }
    
        public static Vector3 GetHandPosition(this Player player, bool compensateVelocity = true, bool validate = true)
        {
            Vector3 offset;
            if (player.IsAimingDownWeapon)
            {
                offset = player.CameraTransform.forward * 0.5f * player.Scale.z + player.CameraTransform.up * -0.05f * player.Scale.y;
            }
            else
            {
                offset = player.CameraTransform.forward * 0.4f * player.Scale.z + new Vector3(0f, -0.18f * player.Scale.y, 0f) + player.CameraTransform.right * 0.1f * player.Scale.x;
            }
            Vector3 handPosition = player.CameraTransform.position + offset;
            if (compensateVelocity)
            {
                handPosition += player.GetVelocityCompensation();
            }

            if (validate && Physics.Raycast(new Ray(player.CameraTransform.position, offset), out var hit, offset.magnitude, 134496257))
            {
                handPosition = hit.point + hit.normal * 0.01f;
            }
            return handPosition;
        }
    
        public static bool HasSightOf(this Player player, Vector3 target)
        {
            return !Physics.Linecast(player.CameraTransform.position, target, 134496257);
        }
    
        public static Vector3 GetVelocityCompensation(this Player player)
        {
            Vector3 velocity = player.Velocity;
            velocity.y = 0f;
            return velocity * 0.13f;
        }
    
        public static Vector3 GetMuzzlePosition(this Player player, float velocityCompensationAmount = 1f, bool validate = true)
        {
            Transform camera = player.CameraTransform;
            Vector3 scale = player.Scale;
            Vector3 offset;
            if (player.IsAimingDownWeapon)
            {
                offset = camera.forward * 0.5f * scale.z + camera.up * -0.05f * scale.y;
            }
            else
            {
                Item currentItem = player.CurrentItem;
                var vector = ((currentItem != null) ? new ItemType?(currentItem.Type) : null) == ItemType.MicroHID ? new Vector3(0.1f, -0.25f, 0.45f) : new Vector3(0.1f, -0.15f, 0.57f);
                offset = vector;
                offset = camera.right * scale.x * offset.x + new Vector3(0f, scale.y * offset.y, 0f) + camera.forward * scale.z * offset.z;
            }
            Vector3 muzzlePosition = camera.position + offset;
            if (velocityCompensationAmount > 0f)
            {
                muzzlePosition += player.GetVelocityCompensation() * velocityCompensationAmount;
            }

            if (validate && Physics.Raycast(new Ray(player.CameraTransform.position, offset), out var hit, offset.magnitude, 134496257))
            {
                muzzlePosition = hit.point + hit.normal * 0.01f;
            }
            return muzzlePosition;
        }
    
        public static Ray ForwardRay(this Player player, float spread = 0f)
        {
            Vector3 direction = player.CameraTransform.forward;
            if (spread > 0f)
            {
                direction = Helpers.GetSpread(spread) * direction;
            }
            return new Ray(player.CameraTransform.position, direction);
        }
    
        public static float DistanceTo(this Player player, Player? target)
        {
            if (!(target != null))
            {
                return float.MaxValue;
            }
            return player.DistanceTo(target.Position);
        }
    
        public static float DistanceTo(this Player? player, Vector3 position)
        {
            if (player == null)
            {
                return float.MaxValue;
            }
            return Vector3.Distance(player.Position, position);
        }

    
        public static bool InFov(this Player player, Vector3 target, float fov = 60f)
        {
            Vector3 directionToTarget = (target - player.CameraTransform.position).normalized;
            return Vector3.Angle(player.CameraTransform.forward.normalized, directionToTarget) <= fov;
        }
    
        public static Color RoleColor(this Player? player)
        {
            if (player == null || !player.IsConnected)
            {
                return new Color(0.1f, 0.1f, 0.1f);
            }
            
            return player.Role.Type.GetColor();
        }
    
        public static bool Sees(this Player player, Vector3 target, float fov = 60f)
        {
            return player.HasSightOf(target) && player.InFov(target, fov);
        }
    
        public static float GetRenderDistanceSqr(this Player player)
        {
            if (player.Position.y <= 800f)
            {
                return 1369f;
            }
            return 5041f;
        }
    }
}