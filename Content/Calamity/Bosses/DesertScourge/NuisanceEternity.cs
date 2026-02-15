using CalamityMod.NPCs.DesertScourge;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasSouls.Content.Bosses.VanillaEternity;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.DesertScourge
{
    public abstract class NuisanceEternity : CalDLCEmodeBehavior
    {
        public override void SetDefaults()
        {
            NPC.lifeMax /= 2;
            NPC.damage = 30;
        }

        public override void ModifyHitByItem(Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            modifiers.FinalDamage *= 0.7f;
        }
        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (projectile.type == ProjectileID.SporeCloud)
            {
                modifiers.FinalDamage.Base = 1;
            }

            DestroyerSegment.PierceResistance(projectile, ref modifiers);
        }
    }
    public class NuisanceHeadEternity : NuisanceEternity
    {
        public override int NPCOverrideID => ModContent.NPCType<DesertNuisanceHead>();
    }
    public class NuisanceBodyEternity : NuisanceEternity
    {
        public override int NPCOverrideID => ModContent.NPCType<DesertNuisanceBody>();
    }
    public class NuisanceTailEternity : NuisanceEternity
    {
        public override int NPCOverrideID => ModContent.NPCType<DesertNuisanceTail>();
    }

    //why are they separate
    public class NuisanceHeadYoungEternity : NuisanceEternity
    {
        public override int NPCOverrideID => ModContent.NPCType<DesertNuisanceHeadYoung>();
    }
    public class NuisanceBodyYoungEternity : NuisanceEternity
    {
        public override int NPCOverrideID => ModContent.NPCType<DesertNuisanceBodyYoung>();
    }
    public class NuisanceTailYoungEternity : NuisanceEternity
    {
        public override int NPCOverrideID => ModContent.NPCType<DesertNuisanceTailYoung>();
    }
}
