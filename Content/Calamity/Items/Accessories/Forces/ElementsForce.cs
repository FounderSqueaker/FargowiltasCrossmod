using CalamityMod;
using CalamityMod.Items.Materials;
using FargowiltasCrossmod.Assets.Particles;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasSouls;
using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Content.UI.Elements;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Forces
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class ElementsForce : BaseForce
    {
        public override string Texture => "FargowiltasCrossmod/Content/Calamity/Items/Accessories/Forces/" + Name;
        public override List<AccessoryEffect> ActiveSkillTooltips =>
            [AccessoryEffectLoader.GetEffect<ElementsForceEffect>()];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(8, 24));
            ItemID.Sets.AnimatesAsSoul[Type] = true;

        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Purple;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<ElementsForceEffect>(Item);
            player.AddEffect<HydrothermicEffect>(Item);
            player.AddEffect<DaedalusEffect>(Item);
            if (player.CalamityAddon().ReaverToggle)
                player.FargoSouls().WingTimeModifier += 0.25f;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient<AerospecEnchant>();
            recipe.AddIngredient<DaedalusEnchant>();
            recipe.AddIngredient<ReaverEnchant>();
            recipe.AddIngredient<HydrothermicEnchant>();
            recipe.AddTile(ModContent.TileType<Fargowiltas.Content.Items.Tiles.CrucibleCosmosSheet>());
            recipe.Register();
        }
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class ElementsHeader : EnchantHeader
    {
        public override int Item => ModContent.ItemType<ElementsForce>();
        public override float Priority => 0.91f;
    }
    public class ElementsForceEffect : AccessoryEffect
    {
        public override Header ToggleHeader => null;
        public override bool ActiveSkill => true;
        public override void ActiveSkillJustPressed(Player player, bool stunned)
        {
            var addon = player.CalamityAddon();
            addon.ReaverToggle = !addon.ReaverToggle;
            SoundEngine.PlaySound(SoundID.Item4, player.Center);
            Color color = addon.ReaverToggle ? Color.LightSteelBlue : Color.Orange;
            for (int i = 0; i < 14; i++)
            {
                ReaverSpark spark = new(new Vector2(player.Center.X + Main.rand.NextFloat(-10, 10), player.Center.Y + Main.rand.NextFloat(-10, 10)), Main.rand.NextVector2Circular(4, 4),
                    color, 0.3f, 20, 10, player.whoAmI);
                spark.Spawn();
            }
        }
        public override void PostUpdateEquips(Player player)
        {
            var addon = player.CalamityAddon();
            if (addon.ReaverToggle) // blizzard mode
            {
                if (player.velocity.Y != 0)
                {
                    addon.ElementsAirTime++;
                    static float DamageFormula(float x) => x / MathF.Sqrt(x * x + 1);
                    float x = addon.ElementsAirTime / 420f;
                    float bonusMultiplier = DamageFormula(x); // This function approaches y = 1 as x approaches infinity.
                    float bonusDamage = bonusMultiplier * 0.5f;
                    player.GetDamage(DamageClass.Generic) += bonusDamage;

                    CooldownBarManager.Activate("AerospecDamage", ModContent.Request<Texture2D>("FargowiltasCrossmod/Content/Calamity/Items/Accessories/Enchantments/AerospecEnchant").Value, new Color(153, 200, 193),
                    () => LumUtils.Saturate(DamageFormula(Main.LocalPlayer.CalamityAddon().ElementsAirTime / 420f)), true, activeFunction: player.HasEffect<ElementsForceEffect>);
                }
                else
                    addon.ElementsAirTime = 0;
                player.lifeRegen += 12;
                player.moveSpeed += 0.3f;
                if (player.miscCounter % 3 == 2 && player.dashDelay > 0)
                    player.dashDelay--;
                player.Calamity().reaverSpeed = true;
            }
            else // magma mode
            {
                player.Calamity().reaverSpeed = true;
                //player.endurance += 0.3f;
                player.statDefense += 20;
                player.noKnockback = true;
            }
        }
    }
}
