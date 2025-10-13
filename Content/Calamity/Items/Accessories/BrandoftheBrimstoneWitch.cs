using CalamityMod;
using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using Fargowiltas.Content.Items.Tiles;
using FargowiltasCrossmod.Content.Calamity.Toggles;
using FargowiltasCrossmod.Core;
using FargowiltasSouls.Content.Items;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using FargowiltasSouls.Content.Items.Materials;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.ModPlayers;
using FargowiltasSouls.Core.Toggler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class BrandoftheBrimstoneWitch : SoulsItem
    {
        //public override string Texture => "FargowiltasSouls/Content/Items/Placeholder";

        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(8, 4));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.value = 1000000;
            Item.rare = ModContent.RarityType<Violet>();

        }
        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {

        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            ActiveEffects(player, Item);
            player.GetDamage(DamageClass.Generic) += 0.1f;
            player.GetCritChance(DamageClass.Generic) += 10;
        }
        public static void ActiveEffects(Player player, Item item)
        {
            // chalice
            player.pStone = true;
            player.lifeRegen += 2;

            var calPlayer = player.Calamity();
            if (player.AddEffect<ChaliceOfTheBloodGodEffect>(item))
            {
                calPlayer.chaliceOfTheBloodGod = true; // +25% hp and chalice effect
            }
            else
            {
                calPlayer.bloodPact = true; // only +25% hp, no chalice effect
            }

            // affliction
            calPlayer.affliction = true;
            if (player.whoAmI != Main.myPlayer && player.miscCounter % 10 == 0)
            {
                if (Main.LocalPlayer.team == player.team && player.team != 0)
                {
                    Main.LocalPlayer.AddBuff(ModContent.BuffType<Afflicted>(), 20, true);
                }
            }

            // calamity
            if (player.AddEffect<CalamityEffect>(item))
            {
                calPlayer.blazingCursorDamage = true;
                calPlayer.blazingCursorVisuals = true;
            }

            // flesh totem
            calPlayer.fleshTotem = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<CalamityMod.Items.Accessories.Calamity>()
                .AddIngredient<ChaliceOfTheBloodGod>()
                .AddIngredient<FleshTotem>()
                .AddIngredient<VoidofCalamity>()
                .AddIngredient<Affliction>()
                .AddIngredient<VoidofExtinction>()

                .AddIngredient<AbomEnergy>(5)
                .AddTile<CrucibleCosmosSheet>()
                .Register();
        }
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public abstract class BotBWEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<BrandoftheBrimstoneWitchHeader>();
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class CalamityEffect : BotBWEffect
    {
        public override int ToggleItemType => ModContent.ItemType<CalamityMod.Items.Accessories.Calamity>();
    }

    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class ChaliceOfTheBloodGodEffect : BotBWEffect
    {
        public override int ToggleItemType => ModContent.ItemType<ChaliceOfTheBloodGod>();
    }



    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class HeartoftheElementsEffect : BotBWEffect
    {
        public override bool IsLoadingEnabled(Mod mod) => false;
        public override int ToggleItemType => ModContent.ItemType<HeartoftheElements>();

        public override bool MinionEffect => true;
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class PurityEffect : BotBWEffect
    {
        public override bool IsLoadingEnabled(Mod mod) => false;
        public override int ToggleItemType => ModContent.ItemType<Radiance>();
        public override bool MutantsPresenceAffects => true;
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class TheSpongeEffect : BotBWEffect
    {
        public override bool IsLoadingEnabled(Mod mod) => false;
        public override int ToggleItemType => ModContent.ItemType<TheSponge>();
        public override bool MutantsPresenceAffects => true;
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class NebulousCoreEffect : BotBWEffect
    {
        public override bool IsLoadingEnabled(Mod mod) => false;
        public override int ToggleItemType => ModContent.ItemType<NebulousCore>();
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class YharimsGiftEffect : BotBWEffect
    {
        public override bool IsLoadingEnabled(Mod mod) => false;
        public override int ToggleItemType => ModContent.ItemType<YharimsGift>();
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class DraedonsHeartEffect : BotBWEffect
    {
        public override bool IsLoadingEnabled(Mod mod) => false;
        public override int ToggleItemType => ModContent.ItemType<DraedonsHeart>();
    }
}