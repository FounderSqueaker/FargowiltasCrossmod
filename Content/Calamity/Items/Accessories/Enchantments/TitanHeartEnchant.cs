using CalamityMod;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Armor.TitanHeart;
using CalamityMod.Items.Tools;
using CalamityMod.Items.Weapons.Rogue;
using FargowiltasCrossmod.Core;
using FargowiltasSouls;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [LegacyName("TitanHeartEnchantment")]
    public class TitanHeartEnchant : BaseEnchant
    {

        public override Color nameColor => new Color(102, 96, 117);

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Pink;
            Item.value = 100000;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<TitanHeartEffect>(Item);
            
        }
        
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient<TitanHeartMask>();
            recipe.AddIngredient<TitanHeartMantle>();
            recipe.AddIngredient<TitanHeartBoots>();
            recipe.AddIngredient<MonolithHammer>();
            recipe.AddIngredient<GacruxianMollusk>();
            recipe.AddRecipeGroup("FargowiltasCrossmod:AnyAstralFruit");
            recipe.AddTile(TileID.CrystalBall);
            recipe.Register();
        }
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class TitanHeartEffect : AccessoryEffect
    {
        public override Header ToggleHeader => null;
        public override int ToggleItemType => ModContent.ItemType<TitanHeartEnchant>();
        
        public override void PostUpdateEquips(Player player)
        {
            var calPlayer = player.Calamity();
            calPlayer.stressPills = true;
            bool wiz = player.ForceEffect<TitanHeartEffect>();

            // dr
            float dr = wiz ? 0.3f : 0.15f;
            if (calPlayer.adrenaline > 0 && calPlayer.adrenaline < calPlayer.adrenalineMax)
                player.endurance += dr * calPlayer.adrenaline / calPlayer.adrenalineMax;
            // charge speed when grounded
            if (player.velocity.Y == 0) // grounded
            {
                // cal adren conditions
                bool wofAndNotHell = Main.wofNPCIndex >= 0 && player.position.Y < (float)(Main.UnderworldLayer * 16);
                if (CalamityPlayer.areThereAnyDamnBosses && calPlayer.AdrenalineEnabled && calPlayer.adrenalinePauseTimer == 0 && !wofAndNotHell)
                {
                    float defaultRate = calPlayer.adrenalineMax / calPlayer.AdrenalineChargeTime; // base cal charge rate, do not change
                    float balanceFactor = 0.5f; // change this to tune charge speed
                    if (wiz)
                        balanceFactor = 0.8f;

                    calPlayer.adrenaline += defaultRate * balanceFactor;

                    // base cal "adren full" trigger
                    if (calPlayer.adrenaline >= calPlayer.adrenalineMax)
                    {
                        calPlayer.adrenaline = calPlayer.adrenalineMax;

                        // Play a sound when the Adrenaline Meter is full
                        if (player.whoAmI == Main.myPlayer && calPlayer.playFullAdrenalineSound)
                        {
                            calPlayer.playFullAdrenalineSound = false;
                            SoundEngine.PlaySound(CalamityPlayer.AdrenalineFilledSound);
                        }
                    }
                    else
                    {
                        calPlayer.playFullAdrenalineSound = true;

                        // dust
                        for (int i = 0; i < 1; i++)
                        {
                            Vector2 pos = player.Bottom;
                            pos.X += Main.rand.NextFloat(-50, 50);
                            Vector2 aimPos = player.Center + Main.rand.NextVector2Circular(player.width / 2, player.width / 2);
                            Vector2 aim = (aimPos - pos) / 10;
                            int d = Dust.NewDust(pos, 0, 0, DustID.PurpleTorch, aim.X, aim.Y, Scale: 2);
                            Main.dust[d].noGravity = true;
                            Main.dust[d].velocity = aim;
                        }
                    }
                }
            }
        }
    }
}
