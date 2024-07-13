using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Audio;
using Terraria.GameContent;
using ChargeBow;
using Microsoft.Xna.Framework.Audio;
using ReLogic.Utilities;

namespace ChargedBow
{
    public class ChargedBowProjectile : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_294";


        private float Rotation, c1, td, aoff, float1, charge;
        private int damage, t, t2, special;
        private Color color = new (0, 0, 0);
        private SpriteEffects sprite;
        private Item item;
        private bool repeater, itemAutoUse;
        private bool load = false;
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 21;
            Projectile.tileCollide = false;
            Projectile.aiStyle = 75;
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.ownerHitCheck = true;
            Projectile.penetrate = -1;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (load == false)
            {
                Main.instance.LoadProjectile((int)Projectile.ai[1]);
                load = true;
            }
            Player player = Main.player[Projectile.owner];
            if (player.channel)
            {
                if (player.direction == 1)
                {
                    Rotation = Vector2.Normalize(Main.MouseWorld - player.MountedCenter).ToRotation();
                    sprite = SpriteEffects.None;
                }
                else
                {
                    Rotation = Vector2.Normalize(player.MountedCenter - Main.MouseWorld).ToRotation() + MathHelper.Pi;
                    sprite = SpriteEffects.FlipVertically;
                }
            }
            if (charge < 40f) color = lightColor;
            else
            {
                td++;
                Color highlight = Color.White;
                Color lowlight = new(18, 18, 18);
                if (td <= 20)
                {
                    color = Color.Lerp(color, highlight, 0.2f);
                }
                else
                {
                    color = Color.Lerp(color, lowlight, 0.2f);
                }
                if (td > 40) td = 0;
            }
            Color drawColor = lightColor;
            if (!itemAutoUse && t2 == 0) drawColor = color;
            #region Bow
            Texture2D texture = TextureAssets.Item[(int)Projectile.ai[0]].Value; 
            Rectangle rectangle = new (0, 0, texture.Width, texture.Height);
            Vector2 origin = rectangle.Size() / 2f;
            Vector2 position = player.MountedCenter + Vector2.One.RotatedBy(Rotation - MathHelper.PiOver4) * 9f;
            Main.EntitySpriteDraw(texture, position - Main.screenPosition, rectangle, drawColor, Rotation, origin, Projectile.scale, sprite);
            #endregion
            #region Arrow
            Texture2D arrowTexture = TextureAssets.Projectile[(int)Projectile.ai[1]].Value;
            Rectangle arrowRectangle = new (0, 0, arrowTexture.Width, arrowTexture.Height);
            Vector2 origin3 = arrowRectangle.Size() / 2f;
            Vector2 arrowPos = player.MountedCenter + Vector2.One.RotatedBy(Rotation - MathHelper.PiOver4) * 16f - new Vector2(aoff, 0).RotatedBy(Rotation);

            int drawCharge = (int)(charge);
            if ((player.channel || repeater) && drawCharge < 40)
            {

                if (++float1 >= 8 && (!Main.autoPause || (Main.autoPause && !Main.playerInventory)))
                {
                    aoff += (byte)(1 * (25 / (item.useTime / 2)));
                    float1 = 0;
                }
            }
            if ((player.channel || repeater) & t2 == 0)
            {
                Main.EntitySpriteDraw(arrowTexture, arrowPos - Main.screenPosition, arrowRectangle, drawColor, Rotation + MathHelper.PiOver2,
                    origin3, Projectile.scale, SpriteEffects.None);
            }
            #endregion
            return false;
        }
        public override void AI()
        {
            #region Settings
            Player player = Main.player[Projectile.owner];
            Projectile.velocity = Vector2.Normalize(Main.MouseWorld - player.MountedCenter);
            Projectile.position = player.MountedCenter + Vector2.One.RotatedBy(Rotation - MathHelper.PiOver4) * 10f;

            item = player.HeldItem;
            Projectile.knockBack = item.knockBack;

            itemAutoUse = (item.type == ItemID.HellwingBow || item.type == ItemID.PulseBow
                || item.type == ItemID.ShadowFlameBow || item.type == ItemID.AdamantiteRepeater || item.type == ItemID.CobaltRepeater
                || item.type == ItemID.HallowedRepeater || item.type == ItemID.MythrilRepeater || item.type == ItemID.OrichalcumRepeater
                || item.type == ItemID.PalladiumRepeater || item.type == ItemID.TitaniumRepeater);

            repeater = item.type == ItemID.AdamantiteRepeater || item.type == ItemID.CobaltRepeater
                || item.type == ItemID.HallowedRepeater || item.type == ItemID.MythrilRepeater || item.type == ItemID.OrichalcumRepeater
                || item.type == ItemID.PalladiumRepeater || item.type == ItemID.TitaniumRepeater;

            bool shoot = (!player.channel && !repeater) || (itemAutoUse && charge == 40f) || (repeater & charge == 40f);
            #endregion
            Vector2 position = player.Center + Vector2.One.RotatedBy(Rotation - MathHelper.PiOver4) * 9f;
            Vector2 speed = new Vector2(item.shootSpeed, 0).RotatedBy(Rotation) * ((charge / 20) * 0.8f);

            if ((player.channel || repeater) & t2 == 0 & charge < 40f)
            {
                Charging(player);
                player.GetModPlayer<PCh>().bow = true;
            }
            /*
            else if (!shoot && charge == 40)
            {
                FullCharge(position);
            }
            */
            else if (shoot && t2 == 0)
            {
                ShootArrow(position, speed);
                player.GetModPlayer<PCh>().bow = false;
            }

            if (t2 == 1)
            {
                player.direction = player.oldDirection;
                t++;
                if (t >= item.useTime / 2) Projectile.Kill();
            }
        }
        #region Private Methods
        private bool start;
        private SlotId sound;
        private void Charging(Player player)
        {
            if (!start)
            {
                SoundStyle bowSound = new SoundStyle("ChargeBow/Assets/Sounds/BowSound");
                sound = SoundEngine.PlaySound(bowSound.WithVolumeScale(Main.soundVolume * 0.4f));

                start = true;
            }
            charge += 25 / (item.useTime / 2);
            if (charge >= 40f)
            {
                if (SoundEngine.TryGetActiveSound(sound, out var s))
                {
                    s.Stop();
                }
                if (c1 == 0 && !itemAutoUse)
                {
                    c1 = 1; SoundEngine.PlaySound(SoundID.MaxMana, player.Center);
                }
                charge = 40f;
            }
            Projectile.timeLeft = 25;
        }
        /*
        private void FullCharge(Vector2 position)
        {
            if (Projectile.ai[1] == ProjectileID.WoodenArrowFriendly && item.type == ItemID.PearlwoodBow)
            {
                t3++;
                if (t3 == 10 || t3 == 20 || t3 == 30)
                {
                    float e = (t3 / 10) - 1;
                    int ranx = Main.rand.Next(-36, 36);
                    int rany = Main.rand.Next(-36, 36);
                    Vector2 pos = position + new Vector2(ranx, rany);
                    for (int d = 0; d < 8; d++)
                    {
                        short type = DustID.BlueFairy;
                        switch (e)
                        {
                            case 0:
                                type = DustID.BlueFairy;
                                break;
                            case 1:
                                type = DustID.GreenFairy;
                                break;
                            case 2:
                                type = DustID.PinkFairy;
                                break;
                        }
                        Dust dust;
                        dust = Dust.NewDustPerfect(pos, type, new Vector2(0, -4).RotatedBy(MathHelper.PiOver4 * d));
                        dust.noGravity = true;
                    }
                    int dmgBoost = (int)e * 7;
                    Projectile.NewProjectile(pos, Vector2.Zero, ModContent.ProjectileType<PearlSpecial>(),
                        item.damage + dmgBoost, 1f, Projectile.owner, e);
                    SoundEngine.PlaySound(SoundID.NPCHit5, Projectile.position);
                }
            }
        }
        */
        private void ShootArrow(Vector2 position, Vector2 speed)
        {
            if(SoundEngine.TryGetActiveSound(sound,out var s))
            {
                s.Stop();
            }
            damage = (int)(item.damage * (charge / 20) * 0.7);

            t2 = 1;
            if (charge >= 40f && Projectile.ai[1] == ProjectileID.WoodenArrowFriendly) special = (int)Projectile.ai[0];

            switch (item.type)
            {
                case ItemID.BeesKnees:
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(),position, speed, (int)Projectile.ai[1], damage, item.knockBack, Projectile.owner, 0, special);
                    SoundEngine.PlaySound(SoundID.Item97, Projectile.position);
                    break;
                case ItemID.ShadowFlameBow:
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), position, speed, (int)Projectile.ai[1], damage, item.knockBack, Projectile.owner, 0, special);
                    SoundEngine.PlaySound(SoundID.Item102, Projectile.position);
                    break;
                default:
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), position, speed, (int)Projectile.ai[1], damage, item.knockBack, Projectile.owner, 0, special);
                    SoundEngine.PlaySound(SoundID.Item5, Projectile.position);
                    break;
            }
        }
        #endregion
        public override bool? CanDamage()
        {
            return false;
        }
    }
}