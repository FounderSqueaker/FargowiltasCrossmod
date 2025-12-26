
using CalamityMod;
using FargowiltasSouls.Core.Systems;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.DataStructures;
using FargowiltasSouls;
using Luminance.Core.Hooking;
using FargowiltasSouls.Core.ModPlayers;
using CalamityMod.CalPlayer;

namespace FargowiltasCrossmod.Core.Calamity.Detours
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class CalDLCPlayerDetours : ModSystem, ICustomDetourProvider
    {
        #pragma warning disable CS8601
        public override void Load()
        {

        }
        void ICustomDetourProvider.ModifyMethods()
        {
            HookHelper.ModifyMethodWithDetour(FargoPlayerPreKill_Method, FargoPlayerPreKill_Detour);

            HookHelper.ModifyMethodWithDetour(EModePlayerPreUpdate_Method, EModePlayerPreUpdate_Detour);

            HookHelper.ModifyMethodWithDetour(CalamityPlayer_CatchFish_Method, CalamityPlayer_CatchFish_Detour);

            HookHelper.ModifyMethodWithDetour(ModifyHurtInfo_CalamityMethod, ModifyHurtInfo_Calamity_Detour);

            HookHelper.ModifyMethodWithDetour(GetAdrenalineDamage_Method, GetAdrenalineDamage_Detour);
        }
        private static readonly MethodInfo FargoPlayerPreKill_Method = typeof(FargoSoulsPlayer).GetMethod("PreKill", LumUtils.UniversalBindingFlags);
        public delegate bool Orig_FargoPlayerPreKill(FargoSoulsPlayer self, double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource);
        internal static bool FargoPlayerPreKill_Detour(Orig_FargoPlayerPreKill orig, FargoSoulsPlayer self, double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            bool retval = orig(self, damage, hitDirection, pvp, ref playSound, ref genGore, ref damageSource);
            if (!retval)
            {
                CalamityPlayer calPlayer = self.Player.Calamity();
                calPlayer.chaliceBleedoutBuffer = 0D;
                calPlayer.chaliceDamagePointPartialProgress = 0D;
                calPlayer.chaliceHitOriginalDamage = 0;
            }
            return retval;
        }

        private static readonly MethodInfo EModePlayerPreUpdate_Method = typeof(EModePlayer).GetMethod("PreUpdate", LumUtils.UniversalBindingFlags);
        public delegate void Orig_EModePlayerPreUpdate(EModePlayer self);
        internal static void EModePlayerPreUpdate_Detour(Orig_EModePlayerPreUpdate orig, EModePlayer self)
        {
            FargoSoulsPlayer soulsPlayer = self.Player.FargoSouls();
            bool antibodies = soulsPlayer.MutantAntibodies;
            if (self.Player.Calamity().oceanCrest || self.Player.Calamity().aquaticEmblem)
            {
                soulsPlayer.MutantAntibodies = true;
            }
            orig(self);
            soulsPlayer.MutantAntibodies = antibodies;
        }

        private static readonly MethodInfo CalamityPlayer_CatchFish_Method = typeof(CalamityPlayer).GetMethod("CatchFish", LumUtils.UniversalBindingFlags);
        public delegate void Orig_CalamityPlayer_CatchFish(CalamityPlayer self, FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition);
        internal static void CalamityPlayer_CatchFish_Detour(Orig_CalamityPlayer_CatchFish orig, CalamityPlayer self, FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition)
        {
            int index = self.Player.FindBuffIndex(BuffID.Gills);
            if (index >= 0) self.Player.buffType[index] = 0;
            orig(self, attempt, ref itemDrop, ref npcSpawn, ref sonar, ref sonarPosition);
            if (index >= 0) self.Player.buffType[index] = BuffID.Gills;

        }
        private static readonly MethodInfo ModifyHurtInfo_CalamityMethod = typeof(CalamityPlayer).GetMethod("ModifyHurtInfo_Calamity", LumUtils.UniversalBindingFlags);
        public delegate void Orig_ModifyHurtInfo_Calamity(CalamityPlayer self, ref Player.HurtInfo info);
        internal static void ModifyHurtInfo_Calamity_Detour(Orig_ModifyHurtInfo_Calamity orig, CalamityPlayer self, ref Player.HurtInfo info)
        {
            bool chalice = self.chaliceOfTheBloodGod;
            if (self.Player.FargoSouls().GuardRaised || self.Player.FargoSouls().MutantPresence)
                self.chaliceOfTheBloodGod = false;
            orig(self, ref info);
            self.chaliceOfTheBloodGod = chalice;
        }

        private static readonly MethodInfo GetAdrenalineDamage_Method = typeof(CalamityUtils).GetMethod("GetAdrenalineDamage", LumUtils.UniversalBindingFlags);
        public delegate float Orig_GetAdrenalineDamage(CalamityPlayer mp);
        internal static float GetAdrenalineDamage_Detour(Orig_GetAdrenalineDamage orig, CalamityPlayer mp)
        {
            float value = orig(mp);
            if (WorldSavingSystem.EternityMode)
                value *= 0.5f;
            return value;
        }
    }
}
