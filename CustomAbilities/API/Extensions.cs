using System.Linq;
using Exiled.API.Features;

namespace CoreLib.CustomAbilities.API
{
    public static class Extensions
    {
        public static Player? GetPlayerByItem(ushort serial) =>
            Player.List.First(x => x.Items.Any(y => y.Serial == serial));
    }
}