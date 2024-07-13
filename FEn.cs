using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using ChargeBow;
using Terraria.Audio;

namespace ChargeBow
{
    public class FEn : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemNoGravity[Type] = true;
            ItemID.Sets.IgnoresEncumberingStone[Type] = true;
            ItemID.Sets.IsAPickup[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 14;
        }
        public override bool ItemSpace(Player player)
        {
            return true;
        }
        Vector2[] oldpos = new Vector2[11]; 
        public override void PostUpdate()
        {
            oldpos[0] = Item.position; 
            Vector2 current = Item.position;
            Vector2 previous = Vector2.Zero;
            for (int i = 1; i < oldpos.Length; i++)
            {
                if (oldpos[i] != Vector2.Zero)
                {
                    previous = oldpos[i];
                    oldpos[i] = current;
                    current = previous;
                }
                if (oldpos[i] == Vector2.Zero)
                {
                    oldpos[i] = current;
                    break;
                }
            }
        }
        public override bool GrabStyle(Player player)
        {
            Vector2 v = Vector2.Normalize(player.Center - Item.Center) * 8f;
            Item.velocity = Vector2.Lerp(Item.velocity, v, 0.08f);
            return true;
        }
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            alphaColor.A = 170;
            Texture2D texture = Mod.Assets.Request<Texture2D>("FEnT").Value;
            Vector2 value = new Vector2(Item.width, Item.height) / 2;
            Color color = Item.GetAlpha(Color.GhostWhite);
            color.A = 200;
            for (int i = 1; i < oldpos.Length; i++)
            {
                Rectangle source = new(0, 0, texture.Width - 5 + (int)Math.Floor((double)(i / 2)) + (int)(Math.Abs(Item.velocity.X) + 3) + (int)(Math.Abs(Item.velocity.Y) + 3), texture.Height);
                Vector2 origin = source.Size() / 2f;
                Vector2 vector = oldpos[i] + value;
                if (!(vector == value))
                {
                    Vector2 vector2 = oldpos[i - 1] + value;
                    float rot;
                    if (i == 1)
                        rot = Item.velocity.ToRotation() - MathHelper.Pi / 2;
                    else
                        rot = (vector2 - vector).ToRotation() - MathHelper.Pi / 2;
                    Color color1 = color * (0.9f - (i * 1.05f) / oldpos.Length);
                    float sca = 1f - (i * 0.05f);
                    spriteBatch.Draw(texture, vector - Main.screenPosition, source, color1, rot + (float)(Math.PI/2), origin, sca, SpriteEffects.None, 0);
                }
            }
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
            Texture2D texture2 = Mod.Assets.Request<Texture2D>("FEn").Value;
            Vector2 value2 = new Vector2(Item.width, Item.height) / 2;
            Color color2 = new(255, 196, 189);
            Rectangle source2 = new(0, 0, texture2.Width, texture2.Height);
            Vector2 origin2 = source2.Size() / 2f;
            Vector2 pos = Item.position + value2 - Main.screenPosition;
            spriteBatch.Draw(texture2, pos, source2, color2, Item.velocity.ToRotation(), origin2, 0.75f, SpriteEffects.None, 1f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            return false;
        }
        public override void GrabRange(Player player, ref int grabRange)
        {
            grabRange += 896;
        }
        public override bool OnPickup(Player player)
        {
            for (int i = 0; i < 10; i++)
            {
                Dust dust = Main.dust[Dust.NewDust(player.Center, 2, 2, DustID.GemRuby)];
                dust.noGravity = true;
            }
            SoundEngine.PlaySound(SoundID.NPCDeath7.WithVolumeScale(Main.soundVolume));
            player.GetModPlayer<PCh>().FiEnValue += 10;
            return false;
        }
    }
}
