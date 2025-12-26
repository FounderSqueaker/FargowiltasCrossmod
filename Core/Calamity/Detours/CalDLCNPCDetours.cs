using CalamityMod.Events;
using CalamityMod.NPCs;
using CalamityMod.World;
using CalamityMod;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Luminance.Core.Hooking;
using FargowiltasSouls.Content.Bosses.Champions.Nature;
using FargowiltasSouls.Content.Bosses.Champions.Terra;
using FargowiltasSouls.Content.Bosses.VanillaEternity;
using CalamityMod.NPCs.Perforator;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.Globals;
using CalamityMod.Systems.Collections;
using CalamityMod.NPCs.SlimeGod;
using CalamityMod.NPCs.Leviathan;

namespace FargowiltasCrossmod.Core.Calamity.Systems
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class CalDLCNPCDetours : ModSystem, ICustomDetourProvider
    {
        #pragma warning disable CS8601

        public override void Load()
        {
            
        }
        void ICustomDetourProvider.ModifyMethods()
        {
            HookHelper.ModifyMethodWithDetour(CalamityPreAIMethod, CalamityPreAI_Detour);
            HookHelper.ModifyMethodWithDetour(CalamityAIMethod, CalamityAI_Detour);
            HookHelper.ModifyMethodWithDetour(CalamityOtherStatChangesMethod, CalamityOtherStatChanges_Detour);
            HookHelper.ModifyMethodWithDetour(CalamityPreDrawMethod, CalamityPreDraw_Detour);
            HookHelper.ModifyMethodWithDetour(CalamityPostDrawMethod, CalamityPostDraw_Detour);
            HookHelper.ModifyMethodWithDetour(CalamityBossHeadSlotMethod, CalamityBossHeadSlot_Detour);

            HookHelper.ModifyMethodWithDetour(NatureChampAIMethod, NatureChampAI_Detour);
            HookHelper.ModifyMethodWithDetour(TerraChampAIMethod, TerraChampAI_Detour);
            HookHelper.ModifyMethodWithDetour(CheckTempleWallsMethod, CheckTempleWalls_Detour);
            HookHelper.ModifyMethodWithDetour(DukeFishronPreAIMethod, DukeFishronPreAI_Detour);
            HookHelper.ModifyMethodWithDetour(MoonLordIsProjectileValid_Method, MoonLordIsProjectileValid_Detour);

            HookHelper.ModifyMethodWithDetour(MediumPerforatorHeadOnKill_Method, MediumPerforatorHeadOnKill_Detour);
            HookHelper.ModifyMethodWithDetour(MediumPerforatorBodyOnKill_Method, MediumPerforatorBodyOnKill_Detour);
            HookHelper.ModifyMethodWithDetour(MediumPerforatorTailOnKill_Method, MediumPerforatorTailOnKill_Detour);

            HookHelper.ModifyMethodWithDetour(EmodeEditSpawnPool_Method, EmodeEditSpawnPool_Detour);
        }
        private static readonly MethodInfo CalamityPreAIMethod = typeof(CalamityGlobalNPC).GetMethod("PreAI", LumUtils.UniversalBindingFlags);
        public delegate bool Orig_CalamityPreAI(CalamityGlobalNPC self, NPC npc);
        internal static bool CalamityPreAI_Detour(Orig_CalamityPreAI orig, CalamityGlobalNPC self, NPC npc)
        {
            bool wasRevenge = CalamityWorld.revenge;
            bool wasDeath = CalamityWorld.death;
            bool wasBossRush = BossRushEvent.BossRushActive;
            bool shouldDisable = CalDLCWorldSavingSystem.E_EternityRev;

            int defDamage = npc.defDamage; // do not fuck with defDamage please

            if (shouldDisable)
            {
                CalamityWorld.revenge = false;
                CalamityWorld.death = false;
                BossRushEvent.BossRushActive = false;
            }
            bool result = orig(self, npc);
            if (shouldDisable)
            {
                CalamityWorld.revenge = wasRevenge;
                CalamityWorld.death = wasDeath;
                BossRushEvent.BossRushActive = wasBossRush;
                npc.defDamage = defDamage; // do not fuck with defDamage please
            }
            return result;
        }
        private static readonly MethodInfo CalamityAIMethod = typeof(CalamityGlobalNPC).GetMethod("AI", LumUtils.UniversalBindingFlags);
        public delegate void Orig_CalamityAI(CalamityGlobalNPC self, NPC npc);

        internal static void CalamityAI_Detour(Orig_CalamityAI orig, CalamityGlobalNPC self, NPC npc)
        {
            bool wasRevenge = CalamityWorld.revenge;
            bool wasDeath = CalamityWorld.death;
            bool wasBossRush = BossRushEvent.BossRushActive;
            bool shouldDisable = CalDLCWorldSavingSystem.E_EternityRev;

            int defDamage = npc.defDamage; // do not fuck with defDamage please

            if (shouldDisable)
            {
                CalamityWorld.revenge = false;
                CalamityWorld.death = false;
                BossRushEvent.BossRushActive = false;
            }
            orig(self, npc);
            if (shouldDisable)
            {
                CalamityWorld.revenge = wasRevenge;
                CalamityWorld.death = wasDeath;
                BossRushEvent.BossRushActive = wasBossRush;
                npc.defDamage = defDamage; // do not fuck with defDamage please
            }
        }
        private static readonly MethodInfo CalamityOtherStatChangesMethod = typeof(CalamityGlobalNPC).GetMethod("OtherStatChanges", LumUtils.UniversalBindingFlags);
        public delegate void Orig_CalamityOtherStatChanges(CalamityGlobalNPC self, NPC npc);

        internal static void CalamityOtherStatChanges_Detour(Orig_CalamityOtherStatChanges orig, CalamityGlobalNPC self, NPC npc)
        {
            orig(self, npc);
            if (!CalDLCWorldSavingSystem.E_EternityRev)
                return;
            switch (npc.type)
            {
                case NPCID.DetonatingBubble:
                    if (NPC.AnyNPCs(NPCID.DukeFishron))
                        npc.dontTakeDamage = false;
                    break;
                default:
                    break;
            }
        }
        private static readonly MethodInfo CalamityPreDrawMethod = typeof(CalamityGlobalNPC).GetMethod("PreDraw", LumUtils.UniversalBindingFlags);
        public delegate bool Orig_CalamityPreDraw(CalamityGlobalNPC self, NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor);

        internal static bool CalamityPreDraw_Detour(Orig_CalamityPreDraw orig, CalamityGlobalNPC self, NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            bool wasRevenge = CalamityWorld.revenge;
            bool wasBossRush = BossRushEvent.BossRushActive;
            bool shouldDisableNPC = new int[] { NPCID.TheDestroyer, NPCID.TheDestroyerBody, NPCID.TheDestroyerTail, NPCID.SkeletronPrime }.Contains(npc.type);
            bool shouldDisable = CalDLCWorldSavingSystem.E_EternityRev && shouldDisableNPC;

            if (shouldDisable)
            {
                CalamityWorld.revenge = false;
                BossRushEvent.BossRushActive = false;
            }
            bool result = orig(self, npc, spriteBatch, screenPos, drawColor);
            if (shouldDisable)
            {
                CalamityWorld.revenge = wasRevenge;
                BossRushEvent.BossRushActive = wasBossRush;
            }
            return result;
        }

        private static readonly List<int> DisablePostDrawNPCS =new()
            {
            NPCID.TheDestroyer,
            NPCID.TheDestroyerBody,
            NPCID.TheDestroyerTail,
            NPCID.WallofFleshEye,
            NPCID.Creeper,
            NPCID.SkeletronPrime
            };
        private static readonly MethodInfo CalamityPostDrawMethod = typeof(CalamityGlobalNPC).GetMethod("PostDraw", LumUtils.UniversalBindingFlags);
        public delegate void Orig_CalamityPostDraw(CalamityGlobalNPC self, NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor);
        internal static void CalamityPostDraw_Detour(Orig_CalamityPostDraw orig, CalamityGlobalNPC self, NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            bool shouldDisableNPC = DisablePostDrawNPCS.Contains(npc.type);
            bool shouldDisable = CalDLCWorldSavingSystem.E_EternityRev && shouldDisableNPC;
            if (shouldDisable)
            {
                return;
            }
            orig(self, npc, spriteBatch, screenPos, drawColor);
        }
        private static readonly MethodInfo CalamityBossHeadSlotMethod = typeof(CalamityGlobalNPC).GetMethod("BossHeadSlot", LumUtils.UniversalBindingFlags);
        public delegate void Orig_CalamityBossHeadSlot(CalamityGlobalNPC self, NPC npc, ref int index);
        internal static void CalamityBossHeadSlot_Detour(Orig_CalamityBossHeadSlot orig, CalamityGlobalNPC self, NPC npc, ref int index)
        {
            bool shouldDisable = CalDLCWorldSavingSystem.E_EternityRev;
            if (shouldDisable)
                return;
            orig(self, npc, ref index);
        }

        private static readonly MethodInfo NatureChampAIMethod = typeof(NatureChampion).GetMethod("AI", LumUtils.UniversalBindingFlags);
        public delegate void Orig_NatureChampAI(NatureChampion self);
        internal static void NatureChampAI_Detour(Orig_NatureChampAI orig, NatureChampion self)
        {
           
            NPC npc = self.NPC;
            double originalSurface = Main.worldSurface;
            if (BossRushEvent.BossRushActive)
            {
                Main.worldSurface = 0;
            }
            orig(self);
            if (BossRushEvent.BossRushActive)
            {
                Main.worldSurface = originalSurface;
            }
        }

        private static readonly MethodInfo TerraChampAIMethod = typeof(TerraChampion).GetMethod("AI", LumUtils.UniversalBindingFlags);
        public delegate void Orig_TerraChampAI(TerraChampion self);
        internal static void TerraChampAI_Detour(Orig_TerraChampAI orig, TerraChampion self)
        {
            NPC npc = self.NPC;
            double originalSurface = Main.worldSurface;
            if (BossRushEvent.BossRushActive)
            {
                Main.worldSurface = 0;
            }
            orig(self);
            if (BossRushEvent.BossRushActive)
            {
                Main.worldSurface = originalSurface;
            }
        }
        private static readonly MethodInfo CheckTempleWallsMethod = typeof(Golem).GetMethod("CheckTempleWalls", LumUtils.UniversalBindingFlags);
        public delegate bool Orig_CheckTempleWalls(Vector2 pos);
        internal static bool CheckTempleWalls_Detour(Orig_CheckTempleWalls orig, Vector2 pos)
        {

            if (BossRushEvent.BossRushActive)
            {
                return true;
            }
            return orig(pos);
        }

        private static readonly MethodInfo DukeFishronPreAIMethod = typeof(DukeFishron).GetMethod("SafePreAI", LumUtils.UniversalBindingFlags);
        public delegate bool Orig_DukeFishronPreAI(DukeFishron self, NPC npc);
        internal static bool DukeFishronPreAI_Detour(Orig_DukeFishronPreAI orig, DukeFishron self, NPC npc)
        {
            if (BossRushEvent.BossRushActive && npc.HasValidTarget)
            {
                Main.player[npc.target].ZoneBeach = true;
            }
            bool result = orig(self, npc);
            if (BossRushEvent.BossRushActive && npc.HasValidTarget)
            {
                Main.player[npc.target].ZoneBeach = false;
            }
            return result;
        }
        private static readonly MethodInfo MoonLordIsProjectileValid_Method = typeof(MoonLord).GetMethod("IsProjectileValid", LumUtils.UniversalBindingFlags);
        public delegate bool Orig_MoonLordIsProjectileValid(MoonLord self, NPC npc, Projectile projectile);
        internal static bool MoonLordIsProjectileValid_Detour(Orig_MoonLordIsProjectileValid orig, MoonLord self, NPC npc, Projectile projectile)
        {
            bool ret = orig(self, npc, projectile);
            if (!Main.player[projectile.owner].buffImmune[ModContent.BuffType<NullificationCurseBuff>()])
            {

                switch (self.GetVulnerabilityState(npc))
                {
                    case 0: //if (!projectile.CountsAsClass(DamageClass.Melee)) return false; break; melee
                        if (projectile.CountsAsClass<RogueDamageClass>())
                            ret = true;
                        break;
                    //case 1: if (!projectile.CountsAsClass(DamageClass.Ranged)) return false; break;
                    //case 2: if (!projectile.CountsAsClass(DamageClass.Magic)) return false; break;
                    //case 3: if (!FargoSoulsUtil.IsSummonDamage(projectile)) return false; break;
                    default: break;
                }
            }
            return ret;
        }
        private static readonly MethodInfo MediumPerforatorHeadOnKill_Method = typeof(PerforatorHeadMedium).GetMethod("OnKill", LumUtils.UniversalBindingFlags);
        public delegate void Orig_MediumPerforatorHeadOnKill(PerforatorHeadMedium self);
        internal static void MediumPerforatorHeadOnKill_Detour(Orig_MediumPerforatorHeadOnKill orig, PerforatorHeadMedium self)
        {
            if (CalDLCWorldSavingSystem.E_EternityRev)
                return;
            orig(self);
        }
        private static readonly MethodInfo MediumPerforatorBodyOnKill_Method = typeof(PerforatorBodyMedium).GetMethod("OnKill", LumUtils.UniversalBindingFlags);
        public delegate void Orig_MediumPerforatorBodyOnKill(PerforatorBodyMedium self);

        internal static void MediumPerforatorBodyOnKill_Detour(Orig_MediumPerforatorBodyOnKill orig, PerforatorBodyMedium self)
        {
            if (CalDLCWorldSavingSystem.E_EternityRev)
                return;
            orig(self);
        }
        private static readonly MethodInfo MediumPerforatorTailOnKill_Method = typeof(PerforatorTailMedium).GetMethod("OnKill", LumUtils.UniversalBindingFlags);
        public delegate void Orig_MediumPerforatorTailOnKill(PerforatorTailMedium self);

        internal static void MediumPerforatorTailOnKill_Detour(Orig_MediumPerforatorTailOnKill orig, PerforatorTailMedium self)
        {
            if (CalDLCWorldSavingSystem.E_EternityRev)
                return;
            orig(self);
        }

        private static readonly MethodInfo EmodeEditSpawnPool_Method = typeof(EModeGlobalNPC).GetMethod("EditSpawnPool", LumUtils.UniversalBindingFlags);
        public delegate void Orig_EmodeEditSpawnPool(EModeGlobalNPC self, IDictionary<int, float> pool, NPCSpawnInfo spawnInfo);

        internal static void EmodeEditSpawnPool_Detour(Orig_EmodeEditSpawnPool orig, EModeGlobalNPC self, IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            var cal = spawnInfo.Player.Calamity();

            bool calamityBiomeZone = cal.ZoneAbyss ||
                cal.ZoneCalamity ||
                cal.ZoneSulphur ||
                cal.ZoneSunkenSea ||
                cal.ZoneAstral && !spawnInfo.Player.PillarZone();
            if (calamityBiomeZone || !pool.TryGetValue(0, out float pool0) || pool0 == 0)
                return;
            orig(self, pool, spawnInfo);

        }
    }
}
