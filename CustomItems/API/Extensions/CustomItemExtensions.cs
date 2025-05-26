using System.Collections.Generic;
using System.Linq;
using CoreLib.CustomItems.API.Features;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;

namespace CoreLib.CustomItems.API.Extensions
{
    public static class CustomItemExtensions
    {
        public static bool IsCustomItem(this ushort serial) => CustomItem.Tracking.ContainsKey(serial);

        public static bool IsCustomItem(this Item? item) => item?.Serial.IsCustomItem() ?? false;
    
        public static bool IsCustomItem(this Pickup? pickup) => pickup?.Serial.IsCustomItem() ?? false;

        public static CustomItem? GetCustomItem(this ushort serial) =>
            CustomItem.Tracking.ContainsKey(serial) ? CustomItem.Tracking[serial] : null;

        public static CustomItem? GetCustomItem(this Item? item) => item?.Serial.GetCustomItem();
    
        public static CustomItem? GetCustomItem(this Pickup? pickup) => pickup?.Serial.GetCustomItem();
    
        public static bool TryGetCustomItem(this Item? item, out CustomItem? customItem)
        {
            customItem = item.GetCustomItem();
            return customItem != null;
        }
    
        public static bool TryGetCustomItem(this Pickup? pickup, out CustomItem? customItem)
        {
            customItem = pickup.GetCustomItem();
            return customItem != null;
        }
    
        public static bool TryGetCustomItem(this ushort serial, out CustomItem? item)
        {
            item = serial.GetCustomItem();
            return item != null;
        }

        public static List<CustomItem> GetAllCustomItemsInInventory(this Player player)
        {
            List<CustomItem> returnList = new();
            returnList.AddRange(player.Items.Where(x => x.IsCustomItem()).Select(item => item.GetCustomItem())!);

            return returnList;
        }
    }
}