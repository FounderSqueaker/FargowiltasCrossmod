using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Graphics.Metaballs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace FargowiltasCrossmod.Content.Calamity.Bosses.CalamitasClone
{
    public class BrimstoneBall : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Boss/CatastrophicCinder";
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void AI()
        {
            Projectile.rotation += 0.2f * Projectile.direction;

            if (Projectile.velocity.Length() < 17)
                Projectile.velocity *= 1.1f;

            Lighting.AddLight(Projectile.Center, 0.25f, 0f, 0f);


            var p = CatastropheMetaball.SpawnParticle(Projectile.Center + Projectile.velocity, -Projectile.velocity, Terraria.GameContent.TextureAssets.Projectile[Type].Width() * 2);
            p.rotation = Projectile.rotation;
            p.TextureToUse = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            p.SizeScaling = 0.5f;

            Projectile.Opacity = 0;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0)
                return;

            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 90);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
            /*
            Color backglowColor = Color.Red;

            Projectile.DrawProjectileWithBackglow(backglowColor, lightColor, 2f);
            return false;
            */
        }
    }
}
