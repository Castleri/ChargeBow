using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Localization;

namespace ChargeBow
{
    public class FireM : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 44;
            Item.maxStack = 1;
            Item.value = Item.buyPrice(0, 0, 0, 75);

        }
        public override void UpdateInventory(Player player)
        {
            player.GetModPlayer<PCh>().FM = true;
        }
    }
}
