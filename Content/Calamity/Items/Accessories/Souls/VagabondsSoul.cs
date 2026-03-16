using CalamityMod;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Weapons.Rogue;
using Fargowiltas.Content.Items.Tiles;
using FargowiltasCrossmod.Content.Calamity.Toggles;
using FargowiltasCrossmod.Core;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.ModPlayers;
using FargowiltasSouls.Core.Toggler;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Souls
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class VagabondsSoul : BaseSoul
    {
        public override string Texture => "FargowiltasCrossmod/Content/Calamity/Items/Accessories/Souls/" + Name;
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage<RogueDamageClass>() += 0.22f;
            player.Calamity().rogueVelocity += 0.2f;
            player.GetCritChance<RogueDamageClass>() += 10;

            player.AddEffect<NanotechEffect>(Item);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Nanotech>()

                .AddIngredient<Valediction>()
                .AddIngredient<GodsParanoia>()
                .AddIngredient<DimensionTearingDisk>()
                .AddIngredient<Wrathwing>()
                .AddIngredient<Seraphim>()

                .AddTile<CrucibleCosmosSheet>()
                .Register();
        }
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class NanotechEffect : UniverseEffect
    {
        public override int ToggleItemType => ModContent.ItemType<Nanotech>();

        public override void PostUpdateEquips(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.nanotech = true;
            modPlayer.raiderTalisman = true;
            modPlayer.electricianGlove = true;
            modPlayer.filthyGlove = true;
            modPlayer.bloodyGlove = true;
        }
    }
}
