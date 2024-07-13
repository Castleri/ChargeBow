using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Security.Permissions;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.DataStructures;
using System;


namespace ChargeBow.Items
{
    public class SoulLant : ModItem
    {
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Type, new Terraria.DataStructures.DrawAnimationVertical(6,3));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; 
        }
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 40;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.holdStyle = ItemHoldStyleID.HoldLamp;
            Item.useStyle = ItemUseStyleID.RaiseLamp;
            Item.maxStack = 1;
            Item.scale = 0.75f; 
            Item.handOffSlot = 1;
            Item.shoot = ModContent.ProjectileType<Projectiles.WispF>();
            Item.UseSound = SoundID.Item82;
        }
        private int soulCount;
        public override bool CanUseItem(Player player)
        {
            int fevalue = player.GetModPlayer<PCh>().FiEnValue;
            return fevalue >= 50 && player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.WispF>()] == 0;
        }
        public override bool? UseItem(Player player)
        {
            int fevalue = player.GetModPlayer<PCh>().FiEnValue;
            if (fevalue >= 50 && player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.WispF>()] == 0)
            {
                int souls = (int)Math.Floor((double)fevalue / 50);
                for (int i=0; i < souls; i++)
                {
                    soulCount++;
                    player.GetModPlayer<PCh>().FiEnValue -= 50;
                }
                return base.UseItem(player);

            } 
            else return false;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < soulCount; i++)
            {
                Vector2 pos = player.itemLocation + new Vector2(8 * player.direction, -10f * player.gravDir);
                pos += new Vector2(Main.rand.Next(-160, 160) * player.gravDir, Main.rand.Next(-160, 160));
                //position += pos;
                Projectile.NewProjectile(Projectile.GetSource_NaturalSpawn(),pos,Vector2.Zero, ModContent.ProjectileType<Projectiles.WispF>(), 30,knockback,player.whoAmI,i);
                for(int j = 0; j < 15; j++)
                {
                    Dust.NewDust(pos, 2, 2, DustID.Torch);
                }
            }
            soulCount = 0;
            return false;
        }
        public override void HoldItem(Player player)
        {
            Vector2 pos = player.itemLocation + new Vector2(8 * Entity.direction, -10f * player.gravDir);
            Color color = new Color(255, 140, 39) * 0.5f;
            Lighting.AddLight(pos, color.ToVector3());
        }
    }
}
