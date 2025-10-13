using System;
using System.Collections.Generic;
using System.Linq;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer;
using CalamityMod.CalPlayer.Dashes;
using CalamityMod.Items;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.PermanentBoosters;
using CalamityMod.Items.Placeables.Furniture;
using CalamityMod.Items.Placeables.Furniture.Fountains;
using CalamityMod.Items.Potions;
using CalamityMod.Items.SummonItems;
using CalamityMod.Items.SummonItems.Invasion;
using CalamityMod.Items.TreasureBags.MiscGrabBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Tiles.Furniture;
using Fargowiltas;
using Fargowiltas.Common.Configs;
using Fargowiltas.Content.Items.Misc;
using Fargowiltas.Content.Items.Summons;
using Fargowiltas.Content.Items.Summons.Deviantt;
using Fargowiltas.Content.Items.Summons.Mutant;
using Fargowiltas.Content.Items.Summons.SwarmSummons;
using Fargowiltas.Content.Items.Summons.VanillaCopy;
using FargowiltasCrossmod.Content.Calamity;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Forces;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Souls;
using FargowiltasCrossmod.Content.Calamity.Items.Summons;
using FargowiltasCrossmod.Content.Calamity.Toggles;
using FargowiltasCrossmod.Core;
using FargowiltasSouls;
using FargowiltasSouls.Common;
using FargowiltasSouls.Content.Items;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Items.Accessories.Eternity;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using FargowiltasSouls.Content.Items.Consumables;
using FargowiltasSouls.Content.Items.Misc;
using FargowiltasSouls.Content.Items.Summons;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.ModPlayers;
using FargowiltasSouls.Core.Systems;
using FargowiltasSouls.Core.Toggler;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasCrossmod.Core.Calamity.Globals
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class CalDLCItemChanges : GlobalItem
    {
        public override bool? UseItem(Item item, Player player)
        {
            FargoSoulsPlayer fargoPlayer = player.FargoSouls();
            CalamityPlayer cplayer = player.Calamity();
            if (item.type == ModContent.ItemType<DeerSinew>())
            {
                player.SetToggleValue<DeerSinewEffect>(false);
            }
            if (item.type == ModContent.ItemType<DeathFruit>())
            {
                if (cplayer.dFruit)
                {
                    cplayer.dFruit = false;
                    player.ConsumedLifeFruit = 20;
                }
                else if (cplayer.eBerry)
                {
                    cplayer.eBerry = false;
                    player.ConsumedLifeFruit = 20;
                }
                else if (cplayer.mFruit)
                {
                    cplayer.mFruit = false;
                    player.ConsumedLifeFruit = 20;
                }
                else if (cplayer.bOrange)
                {
                    cplayer.bOrange = false;
                    player.ConsumedLifeFruit = 20;
                }
            }
            return base.UseItem(item, player);
        }
        public override float UseSpeedMultiplier(Item item, Player player)
        {
            //Mythril stealth fix
            if (player.HasEffect<MythrilEffect>() && item.DamageType == ModContent.GetInstance<RogueDamageClass>() && item.useTime >= item.useAnimation)
            {
                FargoSoulsPlayer modPlayer = player.FargoSouls();
                float ratio = Math.Max((float)modPlayer.MythrilTimer / modPlayer.MythrilMaxTime, 0);
                if (ratio > 0.3)
                {
                    return 0.75f;
                }
            }
            return base.UseSpeedMultiplier(item, player);
        }
        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            FargoSoulsPlayer fargoPlayer = player.FargoSouls();
            CalamityPlayer calPlayer = player.Calamity();

            if (item.type == ModContent.ItemType<EternitySoul>())
            {
                BrandoftheBrimstoneWitch.ActiveEffects(player, item);
            }
            if (item.type == ModContent.ItemType<EternitySoul>() || item.type == ModContent.ItemType<TerrariaSoul>())
            {
                //ModContent.GetInstance<GaleForce>().UpdateAccessory(player, hideVisual);
            }
            if (calPlayer.HasCustomDash || item.type == ModContent.ItemType<CounterScarf>() || item.type == ModContent.ItemType<EvasionScarf>() || item.type == ModContent.ItemType<OrnateShield>()
                || item.type == ModContent.ItemType<AsgardianAegis>() || item.type == ModContent.ItemType<ElysianAegis>() || item.type == ModContent.ItemType<AsgardsValor>()
                || item.type == ModContent.ItemType<StatisNinjaBelt>() || item.type == ModContent.ItemType<StatisVoidSash>() || item.type == ModContent.ItemType<ShieldoftheHighRuler>()
                || item.type == ModContent.ItemType<DeepDiver>() && player.wet || player.Calamity().plaguebringerPatronSet)
            {
                fargoPlayer.HasDash = true;
            }
            if (item.type == ModContent.ItemType<AngelTreads>())
            {
                if (player.AddEffect<ZephyrJump>(item))
                {
                    player.GetJumpState(ExtraJump.FartInAJar).Enable();
                }
            }
            if (item.type == ModContent.ItemType<AeolusBoots>()) // add angel treads effects
            {
                CalamityPlayer modPlayer = player.Calamity();
                modPlayer.angelTreads = true;
                player.accRunSpeed = 7.5f;
                player.moveSpeed += 0.04f; // angel provides 0.12. aeolus provides 0.08. leftover is 0.04
                player.iceSkate = true;
                player.waterWalk = true;
                player.fireWalk = true;
                player.buffImmune[BuffID.OnFire] = true;
            }
            bool dimSoul = item.type == ModContent.ItemType<DimensionSoul>() || item.type == ModContent.ItemType<EternitySoul>();
            bool uniSoul = item.type == ModContent.ItemType<UniverseSoul>() || item.type == ModContent.ItemType<EternitySoul>();

            if (item.type == ModContent.ItemType<SupersonicSoul>() || dimSoul)
            {
                if (player.HasEffect<SupersonicDodge>() && player.EffectItem<SupersonicDodge>() == item)
                {
                    player.brainOfConfusionItem = item;
                    player.blackBelt = true;

                    AccessoryEffectPlayer effectsPlayer = player.AccessoryEffects();
                    int superDodge = ModContent.GetInstance<SupersonicDodge>().Index;
                    effectsPlayer.ActiveEffects[superDodge] = false;
                    effectsPlayer.EffectItems[superDodge] = null;
                    fargoPlayer.SupersonicDodge = false;

                }
                
            }
            if (item.type == ModContent.ItemType<ColossusSoul>() || dimSoul)
            {
                player.buffImmune[ModContent.BuffType<ArmorCrunch>()] = true; // "Stronger" Broken Armor
                player.buffImmune[ModContent.BuffType<BrainRot>()] = true; // Counterpart to Burning Blood
                player.buffImmune[ModContent.BuffType<BurningBlood>()] = true; // "Stronger" Bleeding
                player.buffImmune[BuffID.Venom] = true; // "Stronger" Poisoned
                player.buffImmune[ModContent.BuffType<SulphuricPoisoning>()] = true; // "Stronger" Poisoned
                player.buffImmune[BuffID.Webbed] = true; // "Stronger" Slow
                player.buffImmune[BuffID.Blackout] = true; // "Stronger" Darkness

                // Additional debuff immunities (Everything from Ornate Shield + Asgard's Valor)
                player.buffImmune[BuffID.OnFire] = true;
                player.buffImmune[BuffID.OnFire3] = true;
                player.buffImmune[ModContent.BuffType<BrimstoneFlames>()] = true;
                player.buffImmune[BuffID.Chilled] = true;
                player.buffImmune[BuffID.Frozen] = true;
                player.buffImmune[BuffID.Frostburn] = true;
                player.buffImmune[BuffID.Frostburn2] = true;

                // Additional debuff immunities (Everything from Elysian Aegis + thematic counterparts)
                player.buffImmune[BuffID.CursedInferno] = true;
                player.buffImmune[BuffID.ShadowFlame] = true;
                player.buffImmune[BuffID.Daybreak] = true;
                player.buffImmune[ModContent.BuffType<Nightwither>()] = true;
                player.buffImmune[ModContent.BuffType<HolyFlames>()] = true;

                // Immune to God Slayer Inferno itself
                player.buffImmune[ModContent.BuffType<GodSlayerInferno>()] = true;

                if (player.AddEffect<AsgardianAegisEffect>(item))
                {
                    // Asgardian Aegis ram dash
                    calPlayer.DashID = AsgardianAegisDash.ID;
                    player.dashType = 0;
                    fargoPlayer.HasDash = true;
                }

            }
            if (item.type == ModContent.ItemType<TrawlerSoul>() || dimSoul)
            {
                if (player.AddEffect<AbyssalDivingSuitEffect>(item))
                {
                    if (player.IsUnderwater())
                    {
                        calPlayer.abyssalDivingSuit = true;
                        calPlayer.abyssalDivingSuitHide = hideVisual;
                    }
                    /*
                    player.buffImmune[ModContent.BuffType<AbyssalDivingSuitPlates>()] = true;
                    player.buffImmune[ModContent.BuffType<AbyssalDivingSuitBuff>()] = true;
                    calPlayer.abyssalDivingSuitPlateHits = 3;
                    if (player.IsUnderwater())
                        player.gills = true;
                    calPlayer.abyssalDivingSuitPower = true;
                    calPlayer.depthCharm = true;
                    calPlayer.jellyfishNecklace = true;
                    calPlayer.anechoicPlating = true;
                    calPlayer.ironBoots = true;
                    player.arcticDivingGear = true;
                    player.accFlipper = true;
                    player.accDivingHelm = true;
                    player.iceSkate = true;
                    */
                }
            }
            if (item.type == ModContent.ItemType<WorldShaperSoul>() || dimSoul)
            {
                player.AddEffect<MarniteLasersEffect>(item);
                if (player.ZoneDirtLayerHeight || player.ZoneRockLayerHeight || player.ZoneUnderworldHeight)
                {
                    player.statDefense += 10;
                    player.endurance += 0.05f;
                }
            }

            if (item.type == ModContent.ItemType<BerserkerSoul>() || uniSoul)
            {
                calPlayer.gloveLevel = 6; // gives 0 bonus but overrides the others (does not stack with upgrades)
                player.GetDamage<TrueMeleeDamageClass>() += 0.1f; // the other 10% is from .kbGlove in cal code
                player.GetArmorPenetration<MeleeDamageClass>() += 5; // badge of bravery

                player.AddEffect<ElementalGauntletEffect>(item); // elemental mix
            }
            if (item.type == ModContent.ItemType<SnipersSoul>() || uniSoul)
            {
                calPlayer.rangedAmmoCost *= 0.8f;
            }
            if (item.type == ModContent.ItemType<ArchWizardsSoul>() || uniSoul)
            {
                if (item.type == ModContent.ItemType<ArchWizardsSoul>())
                    player.statManaMax2 += 50;
            }
            if (item.type == ModContent.ItemType<ConjuristsSoul>() || uniSoul)
            {
                player.buffImmune[ModContent.BuffType<Shadowflame>()] = true;
                player.buffImmune[ModContent.BuffType<Irradiated>()] = true;
                if (player.HasEffect<NucleogenesisEffect>() || player.AddEffect<NucleogenesisEffect>(item))
                {
                    fargoPlayer.MinionSlotsNonstack = -1; // kills it from giving any this frame

                    calPlayer.nucleogenesis = true; // +4 minions and onhit effect
                    calPlayer.shadowMinions = true; //shadowflame
                    calPlayer.holyMinions = true; //holy flames
                    calPlayer.voltaicJelly = true; //electrified
                    calPlayer.starTaintedGenerator = true; //astral infection and irradiated

                }
                else
                {
                    if (fargoPlayer.MinionSlotsNonstack < 4 && fargoPlayer.MinionSlotsNonstack >= 0)
                        fargoPlayer.MinionSlotsNonstack = 4; // sums to 4 with base conjurist soul effect

                    calPlayer.nucleogenesis = false; // +4 minions and onhit effect
                    calPlayer.shadowMinions = false; //shadowflame
                    calPlayer.holyMinions = false; //holy flames
                    calPlayer.voltaicJelly = false; //electrified
                    calPlayer.starTaintedGenerator = false; //astral infection and irradiated
                }
            }
            if (uniSoul)
            {
                player.Calamity().rogueVelocity += 0.2f;
                player.AddEffect<NanotechEffect>(item);
            }

            // toggles to Cal accs
            if (calPlayer.rampartOfDeities)
            {
                player.AddEffect<RampartofDeitiesEffect>(item);
                if (!player.HasEffect<RampartofDeitiesEffect>())
                    calPlayer.rampartOfDeities = false;
                //player.AddEffect<DefenseStarEffect>(item);
                //if (!player.HasEffect<DefenseStarEffect>())
                //    player.starCloakItem = null;
                //player.AddEffect<FrozenTurtleEffect>(item);
                //if (!player.HasEffect<FrozenTurtleEffect>())
                //    player.ClearBuff(BuffID.IceBarrier);


            }
        }
        public override bool CanUseItem(Item item, Player player)
        {
            if (item.type == ModContent.ItemType<Masochist>())
                return false;

            if (item.type == ModContent.ItemType<Terminus>())
                return WorldSavingSystem.DownedMutant;

            if (item.type == ModContent.ItemType<CelestialOnion>() && WorldSavingSystem.EternityMode)
                return player.FargoSouls().MutantsPactSlot;

            if (item.type == ModContent.ItemType<SuspiciousLookingChest>())
            {
                //BaseSummon summon = (BaseSummon)item.ModItem;
                if (!Main.hardMode && player.ZoneSnow) return false;
            }
            return true;
        }
        public static bool isFargSummon(Item item) {
            ModItem modI = item.ModItem;
            if (Main.gameMenu)
            {
                return false;
            }
            if (SummonsThatDontMeetConditionsButShould.Contains(item.type))
            {
                return true;
            }
            if (modI != null && (modI is SwarmSummonBase || (modI is BaseSummon summon && (ContentSamples.NpcsByNetId[summon.NPCType].boss || summon.NPCType == NPCID.EaterofWorldsHead))))
            {
                return true;
            }
            return false;
        }
        public static int[] SummonsThatDontMeetConditionsButShould = [ ModContent.ItemType<FleshyDoll>(), ModContent.ItemType<MechanicalAmalgam>(), /*ModContent.ItemType<MechEye>(), */ModContent.ItemType<PortableCodebreaker>(), ModContent.ItemType<CrystallineEffigy>(), ModContent.ItemType<MechLure>(), ModContent.ItemType<CoffinSummon>(), ModContent.ItemType<DevisCurse>(), ModContent.ItemType<AbomsCurse>(), ModContent.ItemType<MutantsCurse>()];
        public override void SetDefaults(Item item)
        {
            if (Fargowiltas.Content.Items.FargoGlobalItem.AlwaysUsableVanillaSummons.Contains(item.type))
            {
                item.useAnimation = item.useTime;
            }
            if (isFargSummon(item))
            {
                //item.maxStack = 999;
                item.consumable = false;
            }
            if (item.type == ItemID.ReaverShark)
            {
                item.pick = 59;
                item.useAnimation = 22;
                item.useTime = 13;
            }
            if (CalDLCSets.GetValue(CalDLCSets.Items.MarniteExclude, item.type) && !FargoGlobalItem.TungstenAlwaysAffects.Contains(item.type))
            {
                FargoGlobalItem.TungstenAlwaysAffects.Add(item.type);
            }
        }
        public override void UpdateInventory(Item item, Player player)
        {
            if (isFargSummon(item))
            {
                //item.maxStack = 1;
                item.consumable = false;
            }
        }
        public override void RightClick(Item item, Player player)
        {
            if (item.type == ModContent.ItemType<StarterBag>())
            {
                WorldSavingSystem.ReceivedTerraStorage = true;
                if (Main.netMode != NetmodeID.SinglePlayer)
                    NetMessage.SendData(MessageID.WorldData);
            }
            base.RightClick(item, player);
        }
        // Copied from Mutant Mod
        static string ExpandedTooltipLoc(string line) => Language.GetTextValue($"Mods.FargowiltasCrossmod.ExpandedTooltips.{line}");
        TooltipLine FountainTooltip(string biome) => new TooltipLine(Mod, "Tooltip0", $"[i:909] [c/AAAAAA:{ExpandedTooltipLoc($"Fountain{biome}")}]");
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            var mutantServerConfig = FargoServerConfig.Instance;

            //if (WorldSavingSystem.EternityMode)
            //{
            //    string notConsumable = Language.GetTextValue("Mods.FargowiltasCrossmod.Items.NotConsumable");
            //    for (int i = 0; i < tooltips.Count; i++)
            //    {
            //        tooltips[i].Text = tooltips[i].Text.Replace("\n" + notConsumable, "");
            //        tooltips[i].Text = tooltips[i].Text.Replace(notConsumable, "");
            //    }
            //}
            for (int i = 0; i < tooltips.Count; i++)
            {
                if (tooltips[i].Text.Contains("30") && item.type == ModContent.ItemType<AstralInjection>())
                {
                    tooltips[i].Text = "";
                }
                if (item.type == ModContent.ItemType<SuspiciousLookingChest>() && tooltips[i].Name == "Tooltip1")
                {
                    tooltips[i].Text += " " + Language.GetTextValue("Conditions.InHardmode");
                }
            }

            int tt0 = tooltips.FindIndex(line => line.Name == "Tooltip0");

            if (item.type == ModContent.ItemType<AbyssalDivingGear>() && WorldSavingSystem.EternityMode)
            {
                foreach (var tooltip in tooltips)
                {
                    if (tooltip.Name == "Tooltip5")
                    {
                        tooltip.Text += "\n" + Language.GetTextValue("Mods.FargowiltasSouls.Items.Extra.SpaceBreathImmunity");
                    }
                }
            }
            if (item.type == ModContent.ItemType<AbyssalDivingSuit>() && WorldSavingSystem.EternityMode)
            {
                foreach (var tooltip in tooltips)
                {
                    if (tooltip.Name == "Tooltip14")
                    {
                        tooltip.Text += "\n" + Language.GetTextValue("Mods.FargowiltasSouls.Items.Extra.SpaceBreathImmunity");
                    }
                }
            }
            if (item.type == ModContent.ItemType<Rock>())
                {
                    tooltips.Add(new TooltipLine(Mod, "sqrl", $"[i:{ModContent.ItemType<TopHatSquirrelCaught>()}] [c/AAAAAA:" + Language.GetTextValue($"Mods.Fargowiltas.ExpandedTooltips.SoldBySquirrel") + $"]"));
                }
            if (item.type == ModContent.ItemType<Masochist>())
            {

                tooltips.RemoveAll(t => t.Text != item.Name);
                tooltips.Add(new TooltipLine(Mod, "MasochistDisabled", ExpandedTooltipLoc("MasoModeDisabled")));
            }
            if (item.type == ModContent.ItemType<Terminus>())
            {
                tooltips.Add(new TooltipLine(Mod, "PostMutant", ExpandedTooltipLoc("UsablePostMutant")));
            }
            if (item.type == ModContent.ItemType<DeerSinew>())
            {
                tooltips.Add(new TooltipLine(Mod, "ToggleDisabledByDefault", ExpandedTooltipLoc("DisabledByDefault")));
            }

            string BalanceLine = Language.GetTextValue($"Mods.FargowiltasCrossmod.EModeBalance.CrossBalanceGeneric");
            if (item.type == ModContent.ItemType<CelestialOnion>() && !Main.masterMode && WorldSavingSystem.EternityMode)
            {
                tooltips.Add(new TooltipLine(Mod, "OnionPactUpgrade", $"[c/FF0000:{BalanceLine}]" + Language.GetTextValue($"Mods.FargowiltasCrossmod.EModeBalance.OnionPackUpgrade")));
            }
            if (item.type == ModContent.ItemType<MutantsPact>() && !Main.masterMode && WorldSavingSystem.EternityMode)
            {
                tooltips.Add(new TooltipLine(Mod, "OnionPactUpgrade", $"[c/FF0000:{BalanceLine}]" + Language.GetTextValue($"Mods.FargowiltasCrossmod.EModeBalance.PactOnionUpgrade")));
            }


            string key = "Mods.FargowiltasCrossmod.Items.AddedEffects.";
            if (item.type == ModContent.ItemType<AeolusBoots>() && !item.social)
            {
                tooltips.Insert(11, new TooltipLine(Mod, "CalAeolus", Language.GetTextValue(key + "AngelTreads")));
            }
            if (item.type == ModContent.ItemType<SupersonicSoul>() && !item.social)
            {
                tooltips[tt0 + 4].Text = Language.GetTextValue(key + "CalSupersonicSoul0");
                //tooltips.Insert(12, new TooltipLine(Mod, "CalSupersonicSoul", Language.GetTextValue(key + "CalamitySupersonic")));
            }
            //Colossus Soul
            if (item.type == ModContent.ItemType<ColossusSoul>() && !item.social)
            {
                tooltips.Insert(tt0 + 2, new TooltipLine(Mod, "CalColossusSoul0", Language.GetTextValue(key + "CalColossusSoul0")));
            }
            if (item.type == ModContent.ItemType<TrawlerSoul>() && !item.social)
            {
                tooltips.Insert(tt0 + 5, new TooltipLine(Mod, "CalTrawlerSoul0", Language.GetTextValue(key + "CalTrawlerSoul0")));
                //tooltips.Insert(8, new TooltipLine(Mod, "CalFishSoul", Language.GetTextValue(key + "CalamityTrawler")));
            }
            if (item.type == ModContent.ItemType<WorldShaperSoul>() && !item.social)
            {
                tooltips.Insert(tt0 + 4, new TooltipLine(Mod, "CalWorldshaperSoul0", Language.GetTextValue(key + "CalWorldshaperSoul0")));
                tooltips.Insert(tt0 + 5, new TooltipLine(Mod, "CalWorldshaperSoul1", Language.GetTextValue(key + "CalWorldshaperSoul1")));
            }

            if (item.type == ModContent.ItemType<BerserkerSoul>() && !item.social && tt0 != -1)
            {
                tooltips[tt0 + 1].Text = tooltips[tt0 + 1].Text + Language.GetTextValue(key + "NoStack"); // if this is a problem for grammar in other languages, let me know.
                tooltips.Insert(tt0 + 2, new TooltipLine(Mod, "CalBerserkerSoul1", Language.GetTextValue(key + "CalBerserkerSoul1")));
                tooltips.Insert(tt0 + 4, new TooltipLine(Mod, "CalBerserkerSoul0", Language.GetTextValue(key + "CalBerserkerSoul0")));
            }

            if (item.type == ModContent.ItemType<SnipersSoul>() && !item.social && tt0 != -1)
            {
                tooltips.Insert(tt0 + 2, new TooltipLine(Mod, "CalSniperSoul0", Language.GetTextValue(key + "CalSniperSoul0")));
            }

            if (item.type == ModContent.ItemType<ArchWizardsSoul>() && !item.social)
            {
                tooltips[tt0 + 2].Text = tooltips[tt0 + 2].Text.Replace("100", "150");
                //tooltips.Insert(8, new TooltipLine(Mod, "CalWizardSoul", Language.GetTextValue(key + "CalamityWizard")));
            }

            if (item.type == ModContent.ItemType<ConjuristsSoul>() && !item.social)
            {
                tooltips[tt0 + 2].Text = tooltips[tt0 + 2].Text.Replace("3", "4");
                tooltips.Insert(tt0 + 5, new TooltipLine(Mod, "ConjuristsSoul0", Language.GetTextValue(key + "CalConjuristsSoul0")));
                tooltips.Insert(tt0 + 6, new TooltipLine(Mod, "ConjuristsSoul1", Language.GetTextValue(key + "CalConjuristsSoul1")));
                //tooltips.Insert(7, new TooltipLine(Mod, "CalConjurSoul", Language.GetTextValue(key + "CalamityConjurist")));
            }

            int expert = tooltips.FindIndex(x => x.Name == "Expert");
            if (item.type == ModContent.ItemType<UniverseSoul>() && !item.social)
            {
                if (SoulsItem.IsNotRuminating(item))
                {
                    var conjurists = "[i:FargowiltasSouls/ConjuristsSoul]";
                    int extraeff = tooltips.FindIndex(t => t.Text.Contains(conjurists));
                    tooltips[extraeff - 1].Text = tooltips[extraeff - 1].Text.Replace("3", "4");
                    tooltips[extraeff].Text = tooltips[extraeff].Text.Replace(conjurists, conjurists + "[i:FargowiltasCrossmod/VagabondsSoul]");
                }
                else
                {
                    var lines = tooltips[tt0].Text.Split("\n").ToList();
                    lines[2] = lines[2].Replace("3", "4");
                    lines.Insert(4, Language.GetTextValue(key + "CalBerserkerSoul0"));
                    lines.Insert(2, Language.GetTextValue(key + "CalBerserkerSoul1"));
                    lines.Insert(3, Language.GetTextValue(key + "CalSniperSoul0"));
                    lines.Insert(4, Language.GetTextValue(key + "CalConjuristsSoul0"));
                    lines.Insert(lines.Count - 1, Language.GetTextValue(key + "CalConjuristsSoul1"));
                    lines.Insert(lines.Count - 1, Language.GetTextValue(key + "CalamityUniverse"));
                    tooltips[tt0].Text = string.Join("\n", lines);
                }
            }

            if (item.type == ModContent.ItemType<DimensionSoul>() && !item.social)
            {
                if (SoulsItem.IsNotRuminating(item))
                {
                    
                }
                else
                {
                    var lines = tooltips[tt0].Text.Split("\n").ToList();
                    lines.Insert(4, Language.GetTextValue(key + "CalColossusSoul0"));
                    lines[5] = Language.GetTextValue(key + "CalDimensionsSoul0");
                    lines.Insert(7, Language.GetTextValue(key + "CalTrawlerSoul0"));
                    lines.Insert(lines.Count - 1, Language.GetTextValue(key + "CalDimensionsSoul1"));
                    tooltips[tt0].Text = string.Join("\n", lines);
                }
            }

            if (FargoClientConfig.Instance.ExpandedTooltips)
            {
                if (mutantServerConfig.Fountains)
                {
                    if (item.type == ModContent.ItemType<AstralFountainItem>())
                        tooltips.Add(FountainTooltip("Astral"));
                    if (item.type == ModContent.ItemType<BrimstoneLavaFountainItem>())
                        tooltips.Add(FountainTooltip("Crags"));
                    if (item.type == ModContent.ItemType<SulphurousFountainItem>())
                        tooltips.Add(FountainTooltip("Sulphur"));
                    if (item.type == ModContent.ItemType<SunkenSeaFountain>())
                        tooltips.Add(FountainTooltip("Sunken"));
                }
            }
        }
    }
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class CalExtraSlotPlayer : ModPlayer
    {
        public bool MutantPactShouldBeEnabled;
        public override void PostUpdate()
        {
            ref bool MutantsPactSlot = ref Player.FargoSouls().MutantsPactSlot;
            if (MutantsPactSlot)
            {
                Player.Calamity().extraAccessoryML = true;
            }
            if (Player.Calamity().extraAccessoryML && !Main.masterMode && WorldSavingSystem.EternityMode)
            {
                if (MutantsPactSlot)
                {
                    MutantPactShouldBeEnabled = true; //store if the slot is enabled
                    //DropPactSlot();
                    MutantsPactSlot = false; //turn it off since celestial onion slot is replacing it
                }
            }
            else if (MutantPactShouldBeEnabled)
            {
                MutantsPactSlot = true;
            }
        }
        public override void SaveData(TagCompound tag)
        {
            tag.Add($"{Mod.Name}.{Player.name}.MutantPactShouldBeEnabled", MutantPactShouldBeEnabled);
        }
        public override void LoadData(TagCompound tag)
        {
            MutantPactShouldBeEnabled = tag.GetBool($"{Mod.Name}.{Player.name}.MutantPactShouldBeEnabled");
        }

        private void DropPactSlot()
        {
            //this is ugly but it has to be like this
            void DropItem(Item item)
            {
                Item.NewItem(Player.GetSource_DropAsItem(), Player.Center, item);
            }
            void DropSlot(ref ModAccessorySlot slot)
            {
                DropItem(slot.FunctionalItem);
                slot.FunctionalItem = new();
                DropItem(slot.VanityItem);
                slot.VanityItem = new();
                DropItem(slot.DyeItem);
                slot.DyeItem = new();
                //making dummy items because nulling ModAccessorySlot items crashes the game because of course it does
            }
            ModAccessorySlot eSlot = LoaderManager.Get<AccessorySlotLoader>().Get(ModContent.GetInstance<EModeAccessorySlot>().Type, Player);
            DropSlot(ref eSlot);
        }
    }
}
