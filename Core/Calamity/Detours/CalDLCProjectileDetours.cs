using CalamityMod.Events;
using CalamityMod.Projectiles;
using CalamityMod.World;
using System;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using FargowiltasSouls;
using Luminance.Core.Hooking;
using CalamityMod.Projectiles.Boss;
using FargowiltasSouls.Content.Projectiles;
using CalamityMod.Projectiles.Ranged;

namespace FargowiltasCrossmod.Core.Calamity.Systems
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class CalDLCProjectileDetours : ModSystem, ICustomDetourProvider
    {
        #pragma warning disable CS8601
        public override void Load()
        {
            
        }
        void ICustomDetourProvider.ModifyMethods()
        {
            HookHelper.ModifyMethodWithDetour(CalamityProjectilePreAIMethod, CalamityProjectilePreAI_Detour);
            HookHelper.ModifyMethodWithDetour(CalamityProjectileCanDamageMethod, CalamityProjectileCanDamage_Detour);

            HookHelper.ModifyMethodWithDetour(BrimstoneMonsterCanHitPlayerMethod, BrimstoneMonsterCanHitPlayer_Detour);

            HookHelper.ModifyMethodWithDetour(FargoSoulsOnSpawnProjMethod, FargoSoulsOnSpawnProj_Detour);
        }
        private static readonly MethodInfo CalamityProjectilePreAIMethod = typeof(CalamityGlobalProjectile).GetMethod("PreAI", LumUtils.UniversalBindingFlags);
        public delegate bool Orig_CalamityProjectilePreAI(CalamityGlobalProjectile self, Projectile projectile);
        internal static bool CalamityProjectilePreAI_Detour(Orig_CalamityProjectilePreAI orig, CalamityGlobalProjectile self, Projectile projectile)
        {
            bool wasRevenge = CalamityWorld.revenge;
            bool wasDeath = CalamityWorld.death;
            bool shouldDisable = CalDLCWorldSavingSystem.E_EternityRev;
            int damage = projectile.damage;
            if (shouldDisable)
            {
                CalamityWorld.revenge = false;
                CalamityWorld.death = false;
            }
            bool result = orig(self, projectile);
            if (shouldDisable)
            {
                CalamityWorld.revenge = wasRevenge;
                CalamityWorld.death = wasDeath;
                projectile.damage = damage;
            }
            return result;
        }

        private static readonly MethodInfo CalamityProjectileCanDamageMethod = typeof(CalamityGlobalProjectile).GetMethod("CanDamage", LumUtils.UniversalBindingFlags);
        public delegate bool? Orig_CalamityProjectileCanDamage(CalamityGlobalProjectile self, Projectile projectile);
        public static bool? CalamityProjectileCanDamage_Detour(Orig_CalamityProjectileCanDamage orig, CalamityGlobalProjectile self, Projectile projectile)
        {
            bool wasRevenge = CalamityWorld.revenge;
            bool wasDeath = CalamityWorld.death;
            bool wasBossRush = BossRushEvent.BossRushActive;
            bool shouldDisable = CalDLCWorldSavingSystem.E_EternityRev;
            if (shouldDisable)
            {
                CalamityWorld.revenge = false;
                CalamityWorld.death = false;
                BossRushEvent.BossRushActive = false;
            }
            bool? result = orig(self, projectile);
            if (shouldDisable)
            {
                CalamityWorld.revenge = wasRevenge;
                CalamityWorld.death = wasDeath;
                BossRushEvent.BossRushActive = wasBossRush;
            }
            return result;
        }

        private static readonly MethodInfo BrimstoneMonsterCanHitPlayerMethod = typeof(BrimstoneMonster).GetMethod("CanHitPlayer", LumUtils.UniversalBindingFlags);
        public delegate bool Orig_BrimstoneMonsterCanHitPlayer(BrimstoneMonster self, Player player);
        internal static bool BrimstoneMonsterCanHitPlayer_Detour(Orig_BrimstoneMonsterCanHitPlayer orig, BrimstoneMonster self, Player player)
        {
            if (self.Type != ModContent.ProjectileType<BrimstoneMonster>())
            {
                return orig(self, player);
            }
            float distSQ = self.Projectile.DistanceSQ(player.Center);
            float radiusSQ = MathF.Pow(170f * self.Projectile.scale, 2);
            if (distSQ > radiusSQ)
                return false;
            return orig(self, player);
        }

        private static readonly MethodInfo FargoSoulsOnSpawnProjMethod = typeof(FargoSoulsGlobalProjectile).GetMethod("OnSpawn", LumUtils.UniversalBindingFlags);
        public delegate void Orig_FargoSoulsOnSpawnProj(FargoSoulsGlobalProjectile self, Projectile projectile, IEntitySource source);
        internal static void FargoSoulsOnSpawnProj_Detour(Orig_FargoSoulsOnSpawnProj orig, FargoSoulsGlobalProjectile self, Projectile proj, IEntitySource source)
        {
            if (proj.type == ModContent.ProjectileType<TitaniumRailgunScope>())
            {
                proj.FargoSouls().CanSplit = false;
            }

            orig(self, proj, source);
        }
    }
}
