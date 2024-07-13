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
            if(item.type == ItemID.WhoopieCushion)
            {
                if(player.GetModPlayer<PCh>().AbilityCooldown == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item16, player.position);
                    player.GetModPlayer<PCh>().AbilityCooldown = 2040;
                    player.GetModPlayer<PCh>().AbilityTimer = 660;
                    player.AddBuff(BuffID.ChaosState, 2040);
                    player.AddBuff(BuffID.WitheredWeapon, 660);
                    switch (Main.rand.Next(0,4))
                    {
                        case 0:
                            Main.NewText("You switch to Heat Mode", Color.OrangeRed);
                            player.inventory[0] = player.GetModPlayer<PCh>().FireInv[0];
                            player.inventory[1] = player.GetModPlayer<PCh>().FireInv[1];
                            for (int i = 3; i < 10; i++)
                            {
                                player.inventory[i].TurnToAir();
                            }
                            break;
                        case 1:
                            Main.NewText("You switch to Cold Mode", Color.LightCyan);
                            player.inventory[0] = player.GetModPlayer<PCh>().IceInv[0];
                            player.inventory[1] = player.GetModPlayer<PCh>().IceInv[1];
                            for (int i = 3; i < 10; i++)
                            {
                                player.inventory[i].TurnToAir();
                            }
                            break;
                        case 2:
                            Main.NewText("You switch to Shock Mode", Color.Violet);
                            player.inventory[0] = player.GetModPlayer<PCh>().ShockInv[0];
                            player.inventory[1] = player.GetModPlayer<PCh>().ShockInv[1];
                            for (int i = 3; i < 10; i++)
                            {
                                player.inventory[i].TurnToAir();
                            }
                            break;
                        case 3:
                            Main.NewText("You switch to Wave Mode", Color.Teal);
                            player.inventory[0] = player.GetModPlayer<PCh>().WaveInv[0];
                            player.inventory[1] = player.GetModPlayer<PCh>().WaveInv[1];
                            for (int i = 3; i < 10; i++)
                            {
                                player.inventory[i].TurnToAir();
                            }
                            break;
                    }
                }
                return player.GetModPlayer<PCh>().AbilityCooldown == 0;
            }
            return base.CanUseItem(item, player);
        }
    }
}
