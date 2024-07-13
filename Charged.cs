﻿using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.GameContent;

namespace Dawn.Items.Weapons.Ranger.Bows.Mechanics
{
    public class Charged : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return true;
        }
        public bool Affected;
        public override void SetDefaults(Item item)
        {
            base.SetDefaults(item);

            Affected = !(item.type == ItemID.HellwingBow || item.type == ItemID.DD2PhoenixBow || item.type == ItemID.DaedalusStormbow
                || item.type == ItemID.Phantasm || item.type == ItemID.Tsunami || item.type == ItemID.DD2BetsyBow || item.type == ItemID.PulseBow
                || item.type == ItemID.ChlorophyteShotbow || item.type == ItemID.BloodRainBow || item.type == ItemID.FairyQueenRangedItem);

            if (item.useAmmo == AmmoID.Arrow && Affected && item.type <= ItemID.JimsDroneVisor)
            {
                item.channel = true;
                item.autoReuse = true;
                item.UseSound = null;
                item.noUseGraphic = true;
                if (item.type == ItemID.PearlwoodBow) item.damage += 10;
            }
            if(item.type == ItemID.Marrow)
            {
                item.damage = 25;
                item.shootSpeed = 9.5f;
                item.useTime = 46;
                item.useAnimation = 46;
                item.knockBack = 8;
            }
            if(item.type == ItemID.BoneSword)
            {

            }
        }
        public override bool CanBeConsumedAsAmmo(Item ammo, Item weapon, Player player)
        {
            if (weapon.useAmmo == AmmoID.Arrow && Affected && weapon.type <= ItemID.JimsDroneVisor) return !(player.ownedProjectileCounts[ModContent.ProjectileType<ChargedBow.ChargedBowProjectile>()] >= 1);
            else return base.CanBeConsumedAsAmmo(ammo, weapon, player);
        }
        public override bool CanUseItem(Item item, Player player)
        {
            if (item.useAmmo == AmmoID.Arrow && Affected && item.type <= ItemID.JimsDroneVisor) return !(player.ownedProjectileCounts[ModContent.ProjectileType<ChargedBow.ChargedBowProjectile>()] >= 1);
            else return base.CanUseItem(item, player);
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (item.useAmmo == AmmoID.Arrow && Affected && item.type <= ItemID.JimsDroneVisor)
            {
                if (player.ownedProjectileCounts[ModContent.ProjectileType<ChargedBow.ChargedBowProjectile>()] == 0)
                {
                    Projectile.NewProjectile(Entity.GetSource_NaturalSpawn(), position.X, position.Y, velocity.X, velocity.Y, ModContent.ProjectileType<ChargedBow.ChargedBowProjectile>(),
                        0, knockback, player.whoAmI, item.type, type);
                }
                return false;
            }
            else if (item.type == ItemID.DaedalusStormbow) return true;
                
            
            else return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
        }
    }
}
