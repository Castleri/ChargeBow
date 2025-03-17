using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace ChargeBow
{
    public class PCh : ModPlayer
    {
        public bool bow;
 
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
    }
}
