using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.Audio;

namespace ChargeBow.Projectiles
{
    public class WispF : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height= 12;
            Projectile.tileCollide = false;
            Projectile.friendly = false;
        }
        bool goingFor;
        bool release;
        int degrees;
        Vector2 mouseSave;
        public override void AI()
        {
            Projectile.timeLeft = 3;
            Projectile.frameCounter++;
            if(Projectile.frameCounter > 3)
            {
                Projectile.frame++;
                if (Projectile.frame >= 5) Projectile.frame = 0;
                Projectile.frameCounter = 0;
            }

            Player player = Main.player[Projectile.owner];
            degrees++;
            if (Projectile.ai[0] == 0)
            {
                if (player.channel && !goingFor)
                {
                    float rot = Vector2.Normalize((Main.MouseWorld - Projectile.position) * player.direction).ToRotation();
                    Projectile.position = Vector2.Lerp(Projectile.position, player.Center + new Vector2(32 * player.direction, -8).RotatedBy(rot), 0.15f);
                    mouseSave = Vector2.Normalize(Main.MouseWorld - Projectile.position);
                    release = true;
                }
                else if (!player.channel && release && !goingFor)
                {
                    Projectile.extraUpdates = 1;
                    Projectile.velocity = mouseSave * 6f;
                    goingFor = true;
                    Projectile.friendly = true;
                    Projectile.tileCollide = true;
                }
                else if (!goingFor) Projectile.velocity = NormalMove(degrees, player.ownedProjectileCounts[ModContent.ProjectileType<WispF>()], player);
            }
            else
            {
                if (player.channel)
                {
                    Projectile.velocity = NormalMove(degrees, player.ownedProjectileCounts[ModContent.ProjectileType<WispF>()], player);
                    release = true;
                }
                else if(!player.channel && release)
                {
                    release = false;
                    Projectile.ai[0]--;
                }
                else Projectile.velocity = NormalMove(degrees, player.ownedProjectileCounts[ModContent.ProjectileType<WispF>()], player);
            }
        }
        private Vector2 NormalMove(int degrees, int soulCount, Player player)
        {
            if (soulCount == 0) soulCount = 1;
            float rot = ((360 / (1 + (soulCount - 1))) * Projectile.ai[0]) + degrees;
            Vector2 pos = player.Center + new Vector2(-8, -6) + new Vector2(0, -42).RotatedBy(MathHelper.ToRadians(rot));
            float dist = Vector2.DistanceSquared(Projectile.position, pos) * 0.125f;
            Vector2 v = Vector2.Normalize(pos - Projectile.position) * MathHelper.Clamp(dist, 0f, 7f);
            return Vector2.Lerp(Projectile.velocity, v, MathHelper.Clamp(dist,0f,0.1f));

        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Mod.Assets.Request<Texture2D>("Projectiles/WispF").Value;
            Vector2 value = new Vector2(Projectile.width, Projectile.height) / 2;
            Rectangle source = new(0, (22 * Projectile.frame), texture.Width, 22);
            Vector2 origin = source.Size() / 2f;
            Vector2 pos = Projectile.position + value - Main.screenPosition;
            DrawData data = new(texture, pos, source, lightColor, 0f, origin, Projectile.scale, SpriteEffects.None);
            Main.EntitySpriteDraw(data);
            return false;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if(Projectile.tileCollide) willExplode = true;
            return base.OnTileCollide(oldVelocity);
        }
        bool willExplode;
        public override void OnKill(int timeLeft)
        {
            if (Projectile.owner == Main.myPlayer && goingFor && willExplode)
            {
                SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode);
                for (int i = 0; i < 10; i++)
                {
                    Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 3f);
                    dust.noGravity = true;
                    dust.velocity *= 5f;
                    dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 2f);
                    dust.velocity *= 3f;
                }

                int explosionRadius = 2;
                int minTileX = (int)(Projectile.Center.X / 16f - explosionRadius);
                int maxTileX = (int)(Projectile.Center.X / 16f + explosionRadius);
                int minTileY = (int)(Projectile.Center.Y / 16f - explosionRadius);
                int maxTileY = (int)(Projectile.Center.Y / 16f + explosionRadius);

                Utils.ClampWithinWorld(ref minTileX, ref minTileY, ref maxTileX, ref maxTileY);

                bool explodeWalls = Projectile.ShouldWallExplode(Projectile.Center, explosionRadius, minTileX, maxTileX, minTileY, maxTileY);
                Projectile.ExplodeTiles(Projectile.Center, explosionRadius, minTileX, maxTileX, minTileY, maxTileY, explodeWalls);
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3,180);
        }
        public override bool CanHitPlayer(Player target)
        {
            target.AddBuff(BuffID.OnFire3, 60);
            return goingFor;
        }

    }
}
