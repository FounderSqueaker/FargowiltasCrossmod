using System.Collections.Generic;
using CalamityMod.Items.Armor.Sulphurous;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using FargowiltasCrossmod.Content.Calamity.Projectiles;
using FargowiltasCrossmod.Content.Calamity.Toggles;
using FargowiltasCrossmod.Core;
using FargowiltasSouls;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [LegacyName("SulphurEnchantment")]
    public class SulphurEnchant : BaseEnchant
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            //return FargowiltasCrossmod.EnchantLoadingEnabled;
            return true;
        }
        public override Color nameColor => new Color(181, 139, 161);

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Green;
            Item.value = 20000;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            AddEffects(player, Item);
        }
        public static void AddEffects(Player player, Item item)
        {
            player.AddEffect<SulphurEffect>(item);
        }
        
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient<SulphurousHelmet>();
            recipe.AddIngredient<SulphurousBreastplate>();
            recipe.AddIngredient<SulphurousLeggings>();
            recipe.AddIngredient<ContaminatedBile>();
            recipe.AddIngredient<CausticCroakerStaff>();
            recipe.AddRecipeGroup("FargowiltasCrossmod:AnyAcidFruit");
            recipe.AddTile(TileID.DemonAltar);
            recipe.Register();
        }
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class SulphurEffect : AccessoryEffect
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            //return FargowiltasCrossmod.EnchantLoadingEnabled;
            return true;
        }
        public override Header ToggleHeader => Header.GetHeader<GaleHeader>();
        public override int ToggleItemType => ModContent.ItemType<SulphurEnchant>();
        public override bool ExtraAttackEffect => true;

        public override void PostUpdateEquips(Player player)
        {
            player.GetJumpState<SulphurJump>().Enable();

            // jank fix to make jungle jump have lower priority
            if (player.HasEffect<JungleJump>() && player.GetJumpState<SulphurJump>().Available)
                player.FargoSouls().CanJungleJump = false;
        }
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class SulphurJump : ExtraJump
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            //return FargowiltasCrossmod.EnchantLoadingEnabled;
            return true;
        }
        public override void UpdateHorizontalSpeeds(Player player)
        {
            player.runAcceleration *= 1.5f;
            player.maxRunSpeed *= 1.25f;
        }
        public override Position GetDefaultPosition()
        {
            return BeforeBottleJumps;
        }
        public override IEnumerable<Position> GetModdedConstraints()
        {
            yield return new Before(ModContent.GetInstance<CalamityMod.ExtraJumps.StatigelJump>());
            yield return new Before(ModContent.GetInstance<CalamityMod.ExtraJumps.SulphurJump>());
        }

        public override float GetDurationMultiplier(Player player)
        {
            return 1.15f;
        }
        public override void OnStarted(Player player, ref bool playSound)
        {
            // jank fix to make jungle junk have lower priority, part 2
            if (player.HasEffect<JungleJump>())
                player.FargoSouls().CanJungleJump = true;

            int bubbleDamage = 80;
            if (player.ForceEffect<SulphurEffect>())
            {
                bubbleDamage = 250;
            }

            int offset = player.height;
            if (player.gravDir == -1f)
                offset = 0;

            for (int i = 0; i < 25; ++i)
            {
                Dust dust = Dust.NewDustPerfect(new Vector2(player.Center.X, player.Center.Y + offset), Main.rand.NextBool(3) ? 75 : 161, new Vector2(-player.velocity.X, 6).RotatedByRandom(MathHelper.ToRadians(50f)) * Main.rand.NextFloat(0.1f, 0.8f), 100, default, Main.rand.NextFloat(1.7f, 2.2f));
                dust.noGravity = true;
                if (dust.type == 161)
                {
                    dust.scale = 1.5f;
                    dust.velocity = new Vector2(Main.rand.NextFloat(-4, 4) + -player.velocity.X * 0.3f, Main.rand.NextFloat(2, 4));
                    dust.noGravity = false;
                    dust.alpha = 190;
                }
            }
            foreach (var bubble in LumUtils.AllProjectilesByID(ModContent.ProjectileType<SulphurBubble>()))
            {
                if (bubble.owner == player.whoAmI)
                    bubble.Kill();
            }
            Vector2 vel = Vector2.UnitY * 2;
            if (player.whoAmI == Main.myPlayer)
            {
                Projectile proj = Projectile.NewProjectileDirect(player.GetSource_EffectItem<SulphurEffect>(), player.Center, vel, ModContent.ProjectileType<SulphurBubble>(), bubbleDamage, 1, player.whoAmI);
                if (player.ForceEffect<SulphurEffect>())
                {
                    for (int i = -1; i <= 1; i += 2)
                        Projectile.NewProjectileDirect(player.GetSource_EffectItem<SulphurEffect>(), player.Center, Vector2.UnitX * i * 6f + vel, ModContent.ProjectileType<SulphurBubble>(), bubbleDamage, 1, player.whoAmI);
                }
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    NetMessage.SendData(MessageID.SyncProjectile, number: proj.whoAmI);
                }
            }
        }
        public override void ShowVisuals(Player player)
        {
            base.ShowVisuals(player);
        }
    }
}
