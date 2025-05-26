using System;
using Exiled.API.Features.Items;
using InventorySystem.Items.Firearms.Attachments;

namespace CoreLib.DevTools.FirearmTools.Extensions
{
    public static class FirearmExtensions
    {
        /// <summary>
        /// This method isn`t supporting shotgun
        /// </summary>
        /// <returns>Reload time of firearm</returns>
        public static float GetReloadingTime(this Firearm firearm)
        {
            if (firearm.Type is ItemType.GunShotgun) throw new Exception("brooooo... dont use this method with shotgun pls");

            float time = 0;

            if (firearm.MagazineAmmo is 0)
            {
                switch (firearm.Type)
                {
                    case ItemType.GunCOM15: time = 2.38f; break;
                    case ItemType.GunCOM18: time = 3.27f; break;
                    case ItemType.GunCrossvec: time = 4.14f; break;
                    case (ItemType)52: time = 5.16f; break;
                    case ItemType.GunFSP9: time = 4.37f; break;
                    case ItemType.GunE11SR: time = 4; break;
                    case ItemType.GunRevolver: time = 3.11f; break;
                    case ItemType.GunAK: time = 3.45f; break;
                    case ItemType.GunLogicer: time = 6.8f; break;
                    case ItemType.GunCom45: time = 2.55f; break;
                    case (ItemType)53: time = 3.76f; break;
                }
            }
            else
            {
                switch (firearm.Type)
                {
                    case ItemType.GunCOM15: time = 1.57f; break;
                    case ItemType.GunCOM18: time = 2.56f; break;
                    case ItemType.GunCrossvec: time = 3.36f; break;
                    case (ItemType)52: time = 4.41f; break;
                    case ItemType.GunE11SR: time = 3.3f; break;
                    case ItemType.GunFSP9: time = 3.26f; break;
                    case ItemType.GunRevolver: time = 3.11f; break;
                    case ItemType.GunAK: time = 3; break;
                    case ItemType.GunLogicer: time = 5.3f; break;
                    case ItemType.GunShotgun: time = 2; break;
                    case ItemType.GunCom45: time = 2.55f; break;
                    case (ItemType)53: time = 2.9f; break;
                }
            }

            foreach (var attachment in firearm.Attachments)
            {
                switch (attachment.Name)
                {
                    case AttachmentName.ExtendedMagFMJ: time += 2; break;
                    case AttachmentName.ExtendedMagAP:
                    case AttachmentName.ExtendedMagJHP:
                        time += firearm.Type is (ItemType)52 ? -0.5f : 1.6f;
                        break;

                }
            }

            return time;
        }
    }
}
