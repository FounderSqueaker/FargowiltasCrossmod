using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using CalamityMod;
using CalamityMod.CalPlayer;
using CalamityMod.Enums;
using CalamityMod.Events;
using CalamityMod.Items.Tools;
using CalamityMod.Items.Weapons.DraedonsArsenal;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.NPCs;
using CalamityMod.NPCs.AquaticScourge;
using CalamityMod.NPCs.AstrumAureus;
using CalamityMod.NPCs.AstrumDeus;
using CalamityMod.NPCs.BrimstoneElemental;
using CalamityMod.NPCs.Bumblebirb;
using CalamityMod.NPCs.CalClone;
using CalamityMod.NPCs.CeaselessVoid;
using CalamityMod.NPCs.Crabulon;
using CalamityMod.NPCs.Cryogen;
using CalamityMod.NPCs.DesertScourge;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.ExoMechs;
using CalamityMod.NPCs.ExoMechs.Apollo;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.NPCs.ExoMechs.Artemis;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using CalamityMod.NPCs.HiveMind;
using CalamityMod.NPCs.Leviathan;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.OldDuke;
using CalamityMod.NPCs.Perforator;
using CalamityMod.NPCs.PlaguebringerGoliath;
using CalamityMod.NPCs.Polterghast;
using CalamityMod.NPCs.ProfanedGuardians;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.Ravager;
using CalamityMod.NPCs.Signus;
using CalamityMod.NPCs.SlimeGod;
using CalamityMod.NPCs.StormWeaver;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.NPCs.TownNPCs;
using CalamityMod.NPCs.Yharon;
using CalamityMod.Projectiles;
using CalamityMod.Systems;
using CalamityMod.Systems.Collections;
using CalamityMod.UI.DraedonSummoning;
using CalamityMod.World;
using Fargowiltas.Items.CaughtNPCs;
using Fargowiltas.NPCs;
using FargowiltasCrossmod.Content.Calamity.Bosses.Crabulon;
using FargowiltasCrossmod.Content.Calamity.Bosses.Cryogen;
using FargowiltasCrossmod.Content.Calamity.Bosses.Perforators;
using FargowiltasCrossmod.Content.Calamity.NPCs;
using FargowiltasCrossmod.Content.Calamity.Toggles;
using FargowiltasSouls;
using FargowiltasSouls.Content.Bosses.AbomBoss;
using FargowiltasSouls.Content.Bosses.BanishedBaron;
using FargowiltasSouls.Content.Bosses.Champions.Cosmos;
using FargowiltasSouls.Content.Bosses.Champions.Earth;
using FargowiltasSouls.Content.Bosses.Champions.Life;
using FargowiltasSouls.Content.Bosses.Champions.Nature;
using FargowiltasSouls.Content.Bosses.Champions.Shadow;
using FargowiltasSouls.Content.Bosses.Champions.Spirit;
using FargowiltasSouls.Content.Bosses.Champions.Terra;
using FargowiltasSouls.Content.Bosses.Champions.Timber;
using FargowiltasSouls.Content.Bosses.Champions.Will;
using FargowiltasSouls.Content.Bosses.DeviBoss;
using FargowiltasSouls.Content.Bosses.Lifelight;
using FargowiltasSouls.Content.Bosses.MutantBoss;
using FargowiltasSouls.Content.Bosses.TrojanSquirrel;
using FargowiltasSouls.Content.Buffs;
using FargowiltasSouls.Content.Buffs.Boss;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Buffs.Souls;
using FargowiltasSouls.Content.Items;
using FargowiltasSouls.Content.NPCs.EternityModeNPCs;
using FargowiltasSouls.Content.Projectiles.Masomode;
using FargowiltasSouls.Core.Systems;
using Luminance;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static CalamityMod.Events.BossRushEvent;
using static FargowiltasCrossmod.Core.Common.Globals.DevianttGlobalNPC;
using static Terraria.ModLoader.ModContent;

namespace FargowiltasCrossmod.Core.Calamity.Systems
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class CalDLCCompatibilityMisc : ModSystem
    {
        public override bool IsLoadingEnabled(Mod mod) => ModCompatibility.Calamity.Loaded;
        #region summonloadingbullshit
        public static bool DownedDS => DownedBossSystem.downedDesertScourge;
        public static bool DownedCrab => DownedBossSystem.downedCrabulon;
        public static bool DownedHM => DownedBossSystem.downedHiveMind;
        public static bool DownedPerf => DownedBossSystem.downedPerforator;
        public static bool DownedSG => DownedBossSystem.downedSlimeGod;
        public static bool DownedCryo => DownedBossSystem.downedCryogen;
        public static bool DownedAS => DownedBossSystem.downedAquaticScourge;
        public static bool DownedBE => DownedBossSystem.downedBrimstoneElemental;
        public static bool DownedCalClone => DownedBossSystem.downedCalamitasClone;
        public static bool DownedAA => DownedBossSystem.downedAstrumAureus;
        public static bool DownedLevi => DownedBossSystem.downedLeviathan;
        public static bool DownedPBG => DownedBossSystem.downedPlaguebringer;
        public static bool DownedRav => DownedBossSystem.downedRavager;
        public static bool DownedDeus => DownedBossSystem.downedAstrumDeus;
        public static bool DownedFuck => DownedBossSystem.downedDragonfolly;
        public static bool DownedGuards => DownedBossSystem.downedGuardians;
        public static bool DownedProvi => DownedBossSystem.downedProvidence;
        public static bool DownedCV => DownedBossSystem.downedCeaselessVoid;
        public static bool DownedSignus => DownedBossSystem.downedSignus;
        public static bool DownedSW => DownedBossSystem.downedStormWeaver;
        public static bool DownedPolter => DownedBossSystem.downedPolterghast;
        public static bool DownedOD => DownedBossSystem.downedBoomerDuke;
        public static bool DownedDoG => DownedBossSystem.downedDoG;
        public static bool DownedYharon => DownedBossSystem.downedYharon;
        public static bool DownedExos => DownedBossSystem.downedExoMechs;
        public static bool DownedScal => DownedBossSystem.downedCalamitas;
        public static bool DownedCragmaw => DownedBossSystem.downedCragmawMire;
        public static bool DownedMauler => DownedBossSystem.downedMauler;
        public static bool DownedNuclear => DownedBossSystem.downedNuclearTerror;
        public static bool DownedGSS => DownedBossSystem.downedGSS;
        public static bool DownedClam => DownedBossSystem.downedCLAM;
        #endregion summonloadingbullshit
        public override void Load()
        {
            Add("Archmage", NPCType<Archmage>());
            Add("SeaKing", NPCType<SeaKing>());
            Add("Bandit", NPCType<Bandit>());
            Add("BrimstoneWitch", NPCType<BrimstoneWitch>());
        }
        public static void Add(string internalName, int id)
        {
            if (FargowiltasCrossmod.Instance == null)
            {
                FargowiltasCrossmod.Instance = GetInstance<FargowiltasCrossmod>();
            }
            CaughtNPCItem item = new(internalName, id);
            FargowiltasCrossmod.Instance.AddContent(item);
            FieldInfo info = typeof(CaughtNPCItem).GetField("CaughtTownies", LumUtils.UniversalBindingFlags);
            Dictionary<int, int> list = (Dictionary<int, int>)info.GetValue(info);
            list.Add(id, item.Type);
            info.SetValue(info, list);
        }
        public override void PostSetupContent()
        {
            if (!ModCompatibility.Calamity.Loaded)
                return;
            #region summons
            Mod mutant = ModLoader.GetMod("Fargowiltas");
            mutant.Call("AddSummon", 1.5f, "FargowiltasCrossmod", "MedallionoftheDesert",
                () => DownedDS, Item.buyPrice(gold: 6));
            mutant.Call("AddSummon", 2.5f, "FargowiltasCrossmod", "OphiocordycipitaceaeSprout",
                () => DownedCrab, Item.buyPrice(gold: 9));
            mutant.Call("AddSummon", 3.5f, "FargowiltasCrossmod", "HiveTumor",
                () => DownedHM, Item.buyPrice(gold: 11));
            mutant.Call("AddSummon", 3.5f, "FargowiltasCrossmod", "RedStainedWormFood",
                () => DownedPerf, Item.buyPrice(gold: 11));
            mutant.Call("AddSummon", 6.5f, "FargowiltasCrossmod", "MurkySludge",
                () => DownedSG, Item.buyPrice(gold: 17));
            mutant.Call("AddSummon", 8.5f, "FargowiltasCrossmod", "CryingKey",
                () => DownedCryo, Item.buyPrice(gold: 30));
            mutant.Call("AddSummon", 9.5f, "FargowiltasCrossmod", "SeeFood",
                () => DownedAS, Item.buyPrice(gold: 40));
            mutant.Call("AddSummon", 10.5f, "FargowiltasCrossmod", "FriedDoll",
                () => DownedBE, Item.buyPrice(gold: 40));
            mutant.Call("AddSummon", 11.5f, "FargowiltasCrossmod", "BlightedEye",
                () => DownedCalClone, Item.buyPrice(gold: 45));
            mutant.Call("AddSummon", 12.5f, "FargowiltasCrossmod", "SirensPearl",
                () => DownedLevi, Item.buyPrice(gold: 55));
            mutant.Call("AddSummon", 12.75f, "FargowiltasCrossmod", "ChunkyStardust",
                () => DownedAA, Item.buyPrice(gold: 55));
            mutant.Call("AddSummon", 13.5f, "FargowiltasCrossmod", "ABombInMyNation",
                () => DownedPBG, Item.buyPrice(gold: 60));
            mutant.Call("AddSummon", 13.75f, "FargowiltasCrossmod", "NoisyWhistle",
                () => DownedRav, Item.buyPrice(gold: 60));
            mutant.Call("AddSummon", 17.5f, "FargowiltasCrossmod", "AstrumCor",
                () => DownedDeus, Item.buyPrice(gold: 85));
            mutant.Call("AddSummon", 18.006f, "FargowiltasCrossmod", "BirbPheromones",
                () => DownedFuck, Item.buyPrice(platinum: 1, gold: 10));
            mutant.Call("AddSummon", 18.007f, "FargowiltasCrossmod", "DefiledShard",
                () => DownedGuards, Item.buyPrice(platinum: 1, gold: 10));
            mutant.Call("AddSummon", 18.008f, "FargowiltasCrossmod", "DefiledCore",
                () => DownedProvi, Item.buyPrice(platinum: 1, gold: 20));
            mutant.Call("AddSummon", 18.0091f, "FargowiltasCrossmod", "RiftofKos",
                () => DownedCV, Item.buyPrice(platinum: 1, gold: 30));
            mutant.Call("AddSummon", 18.0092f, "FargowiltasCrossmod", "WormFoodofKos",
                () => DownedSW, Item.buyPrice(platinum: 1, gold: 30));
            mutant.Call("AddSummon", 18.0093f, "FargowiltasCrossmod", "LetterofKos",
                () => DownedSignus, Item.buyPrice(platinum: 1, gold: 30));
            mutant.Call("AddSummon", 18.0094f, "FargowiltasCrossmod", "PolterplasmicBeacon",
                () => DownedPolter, Item.buyPrice(platinum: 1, gold: 40));
            mutant.Call("AddSummon", 18.0095f, "FargowiltasCrossmod", "BloodyWorm",
                () => DownedOD, Item.buyPrice(platinum: 1, gold: 40));
            mutant.Call("AddSummon", 18.0096f, "FargowiltasCrossmod", "SomeKindofSpaceWorm",
                () => DownedDoG, Item.buyPrice(platinum: 2));
            mutant.Call("AddSummon", 18.0097f, "FargowiltasCrossmod", "DragonEgg",
                () => DownedYharon, Item.buyPrice(platinum: 2, gold: 50));
            mutant.Call("AddSummon", 18.012f, "FargowiltasCrossmod", "PortableCodebreaker",
                () => DownedExos, Item.buyPrice(platinum: 3));
            mutant.Call("AddSummon", 18.014f, "FargowiltasCrossmod", "EyeofExtinction",
                () => DownedScal, Item.buyPrice(platinum: 3));
            #endregion summons
            #region bossrush
            //EXPLANATION OF TUPLES
            //int: npc id
            //int: time to set to (1 = day, -1 = night, 0 = dont force time)
            //Action<int>: "boss spawning function" idfk look at cal source but dont put null though
            //int: cooldown (to spawn i think???)
            //bool: boss needs special sound effect
            //float: dimness factor to be used when boss spawned
            //int[]: list of npc ids to not delete when spawning
            //int[]: list of other npc ids that need to die to continue event

            Bosses =
                [
                new Boss(NPCID.KingSlime, spawnContext: type => {
                    NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, type);

                    
                },permittedNPCs: new int[] { NPCID.BlueSlime, NPCID.YellowSlime, NPCID.PurpleSlime, NPCID.RedSlime, NPCID.GreenSlime, NPCID.RedSlime,
                    NPCID.IceSlime, NPCID.UmbrellaSlime, NPCID.Pinky, NPCID.SlimeSpiked, NPCID.RainbowSlime, NPCType<KingSlimeJewelRuby>(),
                     }),

                new Boss(NPCID.MoonLordCore, spawnContext: type =>{
                    NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, type);
                    // When Moon Lord spawns, Boss Rush is considered to be started at least once.
                    // King Slime will then be skipped
                    DownedBossSystem.startedBossRushAtLeastOnce = true;
                }, permittedNPCs: [NPCID.MoonLordLeechBlob, NPCID.MoonLordHand, NPCID.MoonLordHead, NPCID.MoonLordFreeEye]),
                new Boss(NPCType<Providence>(), TimeChangeContext.Day, type =>{
                    SoundEngine.PlaySound(Providence.SpawnSound, Main.player[ClosestPlayerToWorldCenter].Center);
                    int provi = NPC.NewNPC(new EntitySource_WorldEvent(), (int)(Main.player[ClosestPlayerToWorldCenter].Center.X), (int)(Main.player[ClosestPlayerToWorldCenter].Center.Y - 400), type, 1);
                    Main.npc[provi].timeLeft *= 20;
                    CalamityUtils.BossAwakenMessage(provi);
                }, usesSpecialSound: true, permittedNPCs: [NPCType<ProvSpawnDefense>(), NPCType<ProvSpawnHealer>(), NPCType<ProvSpawnOffense>(),
                    NPCType<ProfanedGuardianCommander>(), NPCType<ProfanedGuardianDefender>(), NPCType<ProfanedGuardianHealer>()]),
                

                new Boss(NPCType<Polterghast>(), permittedNPCs: [NPCType<PhantomFuckYou>(), NPCType<PolterghastHook>(), NPCType<PolterPhantom>()]),
                new Boss(NPCType<OldDuke>(), spawnContext: type => {
                    int od = NPC.NewNPC(new EntitySource_WorldEvent(), (int)(Main.player[ClosestPlayerToWorldCenter].Center.X + Main.rand.Next(-100, 101)), (int)Main.player[ClosestPlayerToWorldCenter].Center.Y - 300, type, 1);
                    CalamityUtils.BossAwakenMessage(od);
                    Main.npc[od].timeLeft *= 20;
                }, permittedNPCs: [NPCType<SulphurousSharkron>(), NPCType<OldDukeToothBall>()]),
                new Boss(NPCType<DevourerofGodsHead>(), spawnContext: type => {
                    SoundEngine.PlaySound(DevourerofGodsHead.SpawnSound, Main.player[ClosestPlayerToWorldCenter].Center);
                    NPC.SpawnOnPlayer(ClosestPlayerToWorldCenter, type);
                }, usesSpecialSound: true, permittedNPCs: [NPCType<DevourerofGodsBody>(), NPCType<DevourerofGodsTail>(), 
                NPCType<Signus>(), NPCType<CeaselessVoid>(), NPCType<StormWeaverHead>(), NPCType<StormWeaverBody>(), NPCType<StormWeaverTail>()]),
                new Boss(NPCType<CosmosChampion>(), spawnContext: type => {
                    int erd = NPC.NewNPC(new EntitySource_WorldEvent(), (int)(Main.player[ClosestPlayerToWorldCenter].Center.X), (int)(Main.player[ClosestPlayerToWorldCenter].Center.Y - 400), type, 1);
                    Main.npc[erd].timeLeft *= 20;
                    CalamityUtils.BossAwakenMessage(erd);
                }),
                new Boss(NPCType<Yharon>(), permittedNPCs: NPCType<Dragonfolly>()),
                new Boss(NPCType<AbomBoss>()),
                new Boss(NPCType<Draedon>(), spawnContext: type =>
                {
                    if (!NPC.AnyNPCs(NPCType<Draedon>()))
                    {
                        Player player = Main.player[ClosestPlayerToWorldCenter];

                        SoundEngine.PlaySound(CodebreakerUI.SummonSound, player.Center);
                        Vector2 spawnPos = player.Center + new Vector2(-8f, -100f);
                        int draedon = NPC.NewNPC(new EntitySource_WorldEvent("CalamityMod_BossRush"), (int)spawnPos.X, (int)spawnPos.Y, NPCType<Draedon>());
                        Main.npc[draedon].timeLeft *= 20;
                    }
                }, usesSpecialSound: true, permittedNPCs: new int[] { NPCType<Apollo>(), NPCType<AresBody>(), NPCType<AresGaussNuke>(), NPCType<AresLaserCannon>(), NPCType<AresPlasmaFlamethrower>(), NPCType<AresTeslaCannon>(), NPCType<Artemis>(), NPCType<ThanatosBody1>(), NPCType<ThanatosBody2>(), NPCType<ThanatosHead>(), NPCType<ThanatosTail>() }),
                new Boss(NPCType<SupremeCalamitas>(), spawnContext: type => {
                    SoundEngine.PlaySound(SupremeCalamitas.SpawnSound, Main.player[ClosestPlayerToWorldCenter].Center);
                    CalamityUtils.SpawnBossBetter(Main.player[ClosestPlayerToWorldCenter].Top - new Vector2(42, 84f), type);
                }, dimnessFactor: 0.5f, permittedNPCs: [NPCType<SepulcherArm>(), NPCType<SepulcherBody>(), NPCType<SepulcherHead>(), NPCType<SepulcherTail>(), NPCType<SepulcherBodyEnergyBall>(), NPCType<SoulSeekerSupreme>(), NPCType<BrimstoneHeart>(), NPCType<SupremeCataclysm>(), NPCType<SupremeCatastrophe>()]),
                new Boss(NPCType<MutantBoss>(), permittedNPCs: [NPCType<MutantIllusion>()])
                ];
            
            
            BossDeathEffects.Remove(NPCType<SupremeCalamitas>());
            BossDeathEffects.Remove(NPCType<DevourerofGodsHead>());
            BossDeathEffects.Add(NPCType<MutantBoss>(), npc => { BossRushDialogueSystem.StartDialogue(DownedBossSystem.downedBossRush ? BossRushDialoguePhase.EndRepeat : BossRushDialoguePhase.End); });

            ////Adding bosses to boss rush
            #endregion bossrush
            Mod cal = ModCompatibility.Calamity.Mod;
            cal.Call("RegisterModCooldowns", FargowiltasCrossmod.Instance);
            cal.Call("AddDifficultyToUI", new EternityRevDifficulty());
            cal.Call("AddDifficultyToUI", new MasoDeathDifficulty());

            #region CalDebuffListCompat
            CalamityBuffSets.IsDebuff[BuffType<AnticoagulationBuff>()] = true;
            CalamityBuffSets.IsDebuff[BuffType<AntisocialBuff>()] = true;
            CalamityBuffSets.IsDebuff[BuffType<AtrophiedBuff>()] = true;
            CalamityBuffSets.IsDebuff[BuffType<BerserkedBuff>()] = true;
            CalamityBuffSets.IsDebuff[BuffType<BloodthirstyBuff>()] = true;
            CalamityBuffSets.IsDebuff[BuffType<BaronsBurdenBuff>()] = true;
            CalamityBuffSets.IsDebuff[BuffType<ClippedWingsBuff>()] = true;
            CalamityBuffSets.IsDebuff[BuffType<CrippledBuff>()] = true;
            CalamityBuffSets.IsDebuff[BuffType<CurseoftheMoonBuff>()] = true;
            CalamityBuffSets.IsDebuff[BuffType<DefenselessBuff>()] = true;
            CalamityBuffSets.IsDebuff[BuffType<FlamesoftheUniverseBuff>()] = true;
            CalamityBuffSets.IsDebuff[BuffType<FlippedBuff>()] = true;
            CalamityBuffSets.IsDebuff[BuffType<FusedBuff>()] = true;
            CalamityBuffSets.IsDebuff[BuffType<GodEaterBuff>()] = true;
            CalamityBuffSets.IsDebuff[BuffType<GuiltyBuff>()] = true;
            CalamityBuffSets.IsDebuff[BuffType<HexedBuff>()] = true;
            CalamityBuffSets.IsDebuff[BuffType<HypothermiaBuff>()] = true;
            CalamityBuffSets.IsDebuff[BuffType<InfestedBuff>()] = true;
            CalamityBuffSets.IsDebuff[BuffType<IvyVenomBuff>()] = true;
            CalamityBuffSets.IsDebuff[BuffType<JammedBuff>()] = true;
            CalamityBuffSets.IsDebuff[BuffType<LethargicBuff>()] = true;
            CalamityBuffSets.IsDebuff[BuffType<LightningRodBuff>()] = true;
            CalamityBuffSets.IsDebuff[BuffType<LivingWastelandBuff>()] = true;
            CalamityBuffSets.IsDebuff[BuffType<LovestruckBuff>()] = true;
            CalamityBuffSets.IsDebuff[BuffType<LowGroundBuff>()] = true;
            CalamityBuffSets.IsDebuff[BuffType<MarkedforDeathBuff>()] = true;
            CalamityBuffSets.IsDebuff[BuffType<MidasBuff>()] = true;
            CalamityBuffSets.IsDebuff[BuffType<MutantNibbleBuff>()] = true;
            CalamityBuffSets.IsDebuff[BuffType<NanoInjectionBuff>()] = true;
            CalamityBuffSets.IsDebuff[BuffType<NeurotoxinBuff>()] = true;
            CalamityBuffSets.IsDebuff[BuffType<NullificationCurseBuff>()] = true;
            CalamityBuffSets.IsDebuff[BuffType<OceanicMaulBuff>()] = true;
            CalamityBuffSets.IsDebuff[BuffType<OiledBuff>()] = true;
            CalamityBuffSets.IsDebuff[BuffType<PurgedBuff>()] = true;
            CalamityBuffSets.IsDebuff[BuffType<PurifiedBuff>()] = true;
            CalamityBuffSets.IsDebuff[BuffType<ReverseManaFlowBuff>()] = true;
            CalamityBuffSets.IsDebuff[BuffType<RottingBuff>()] = true;
            CalamityBuffSets.IsDebuff[BuffType<SmiteBuff>()] = true;
            CalamityBuffSets.IsDebuff[BuffType<ShadowflameBuff>()] = true;
            CalamityBuffSets.IsDebuff[BuffType<SqueakyToyBuff>()] = true;
            CalamityBuffSets.IsDebuff[BuffType<StunnedBuff>()] = true;
            CalamityBuffSets.IsDebuff[BuffType<SwarmingBuff>()] = true;
            CalamityBuffSets.IsDebuff[BuffType<TimeFrozenBuff>()] = true;
            CalamityBuffSets.IsDebuff[BuffType<UnluckyBuff>()] = true;
            CalamityBuffSets.IsDebuff[BuffType<UnstableBuff>()] = true;
            CalamityBuffSets.IsDebuff[BuffType<BerserkerInstallBuff>()] = true;
            CalamityBuffSets.IsDebuff[BuffType<HolyPriceBuff>()] = true;
            CalamityBuffSets.IsDebuff[BuffType<BrainOfConfusionBuff>()] = true;
            CalamityBuffSets.IsDebuff[BuffType<LihzahrdCurseBuff>()] = true;
            CalamityBuffSets.IsDebuff[BuffType<RushJobBuff>()] = true;
            CalamityBuffSets.IsDebuff[BuffType<TwinsInstallBuff>()] = true;
            CalamityBuffSets.IsDebuff[BuffType<SnowstormCDBuff>()] = true;
            CalamityBuffSets.IsDebuff[BuffType<HellFireBuff>()] = true;
            CalamityBuffSets.IsDebuff[BuffType<LeadPoisonBuff>()] = true;
            CalamityBuffSets.IsDebuff[BuffType<OriPoisonBuff>()] = true;
            CalamityBuffSets.IsDebuff[BuffType<PungentGazeBuff>()] = true;
            CalamityBuffSets.IsDebuff[BuffType<SolarFlareBuff>()] = true;
            #endregion CalDebuffListCompat
            #region SwordRework
            int[] CalSwordsToApplyRework = [ItemType<AbsoluteZero>(), ItemType<AegisBlade>(),
             ItemType<AnarchyBlade>(),
            ItemType<AstralScythe>(),ItemType<Ataraxia>(),ItemType<Avalanche>(),
            ItemType<BlightedCleaver>(),ItemType<Brimlash>(),ItemType<BrimstoneSword>(),
            ItemType<BrinyBaron>(),ItemType<BurntSienna>(),ItemType<Carnage>(),
            ItemType<TrueCausticEdge>(),ItemType<CelestialClaymore>(),
            ItemType<DarklightGreatsword>(),ItemType<DefiledGreatsword>(),
            ItemType<DraconicDestruction>(),
            ItemType<EssenceFlayer>(),
            ItemType<EvilSmasher>(),
             ItemType<FeralthornClaymore>(),ItemType<FlarefrostBlade>(),
            ItemType<Floodtide>(),ItemType<ForsakenSaber>(),
            ItemType<GaelsGreatsword>(),ItemType<GeliticBlade>(),
            ItemType<Greentide>(),ItemType<HellfireFlamberge>(),
            ItemType<LifehuntScythe>(),ItemType<LionHeart>(),
            ItemType<PerfectDark>(),
            ItemType<RedSun>()
            ,ItemType<SeashineSword>(),ItemType<SolsticeClaymore>(),ItemType<SoulHarvester>(),
            ItemType<StormRuler>(),ItemType<StormSaber>(),
            ItemType<Swordsplosion>(),ItemType<TaintedBlade>(),ItemType<TeardropCleaver>(),
            ItemType<TerrorBlade>(),ItemType<TheDarkMaster>(),
            ItemType<TheLastMourning>(),ItemType<TitanArm>(),
            ItemType<UltimusCleaver>(),ItemType<VeinBurster>(),ItemType<Virulence>(),
            ItemType<VoidEdge>(),ItemType<WindBlade>()];
            SwordGlobalItem.AllowedModdedSwords = SwordGlobalItem.AllowedModdedSwords.Union(CalSwordsToApplyRework).ToArray();
            #endregion
            
        }
        //make this a property instead of directly using it so tml doesnt shit itself trying to load it
        public ref Dictionary<int, Action<NPC>> DeathEffectsList => ref BossRushEvent.BossDeathEffects;
    }
}
