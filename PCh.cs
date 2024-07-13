using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace ChargeBow
{
    public class PCh : ModPlayer
    {
        public bool bow,FM,changeInv;
        public int FiEnValue, AbilityTimer, AbilityCooldown, InvSelect;
        public Item[] MainInv = new Item[3];
        public Item[] FireInv = new Item[2];
        public Item[] IceInv = new Item[2];
        public Item[] ShockInv = new Item[2];
        public Item[] WaveInv = new Item[2];

        public override void Initialize()
        {
            FiEnValue = 0;
            AbilityTimer = 0;
            AbilityCooldown = 0;
            InvSelect = 0;

            MainInv[0] = new Item(ItemID.DiamondStaff, 1, PrefixID.Manic);
            MainInv[1] = new Item(ItemID.Terragrim, 1, PrefixID.Godly);
            MainInv[2] = new Item(ItemID.WhoopieCushion);   

            FireInv[0] = new Item(ItemID.FlowerofFire, 1, PrefixID.Ignorant);
            FireInv[1] = new Item(ItemID.Arkhalis, 1, PrefixID.Godly);

            IceInv[0] = new Item(ItemID.FrostStaff, 1, PrefixID.Annoying);
            IceInv[1] = new Item(ItemID.Arkhalis, 1, PrefixID.Godly);

            ShockInv[0] = new Item(ItemID.CrystalSerpent, 1, PrefixID.Demonic);
            ShockInv[1] = new Item(ItemID.Arkhalis, 1, PrefixID.Godly);

            WaveInv[0] = new Item(5065, 1, PrefixID.Broken); 
            WaveInv[1] = new Item(ItemID.Arkhalis, 1, PrefixID.Godly);

            
        }   

        public override void ResetEffects()
        {
            FM = false;
        }
        public override void PostUpdateRunSpeeds()
        {
            base.PostUpdateRunSpeeds();
            if (bow)
            {
                Player.maxRunSpeed *= 0.3f;
                Player.accRunSpeed = 0.1f;
                Player.jumpHeight = 4;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (FM)
            {
                Item.NewItem(target.GetSource_GiftOrReward(), target.Hitbox, ModContent.ItemType<FEn>());
            }
        }
        public override void PreUpdate()
        {
            if (AbilityCooldown != 0) AbilityCooldown--;
            if (AbilityCooldown == 1) CombatText.NewText(Player.Hitbox, Color.Yellow, "Ability Ready to Use");
            if (AbilityTimer != 0) AbilityTimer--;
            if (AbilityTimer == 1)
            {
                for (int i = 3; i < 10; i++)
                {
                    Player.inventory[i].TurnToAir();
                }
                for (int i = 0; i < 3; i++)
                {
                    Player.inventory[i] = MainInv[i];
                }
            }
        }
    }
}
