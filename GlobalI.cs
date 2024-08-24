using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.Audio;

namespace ChargeBow
{
    public class GlobalI : GlobalItem
    {
        public override bool CanUseItem(Item item, Player player)
        {
            return base.CanUseItem(item, player);
        }
    }
}
