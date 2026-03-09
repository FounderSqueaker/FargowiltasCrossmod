using CalamityMod;
using CalamityMod.Buffs;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Events;
using CalamityMod.NPCs;
using CalamityMod.Systems;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasSouls;
using FargowiltasSouls.Content.Buffs.Souls;
using FargowiltasSouls.Content.Projectiles.BossWeapons;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Core.Calamity.Globals
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class CalDLCAddonGlobalNPC : GlobalNPC
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            //return FargowiltasCrossmod.EnchantLoadingEnabled;
            return true;
        }
        public override bool InstancePerEntity => true;
        public int WulfrumScanned = -1;
        public int PBGDebuffTag = 0;
        public int taggedByPlayer = -1;

        public override void ResetEffects(NPC npc)
        {
            if (PBGDebuffTag > 0) PBGDebuffTag--;
        }
        public static bool HasAnyDoTDebuff(NPC npc)
        {
            for (int i = 0; i < BuffLoader.BuffCount; i++)
            {
                if (CalDLCSets.Buffs.DoTDebuff[i] && npc.HasBuff(i))
                {
                    return true;
                }
            }
            return false;
        }

        //Hardmode enchant. not in release.
        public override bool PreAI(NPC npc)
        {
            /*if (PBGDebuffTag > 0 && FargowiltasCrossmod.EnchantLoadingEnabled)
            {
                int distance = 300;
                if (taggedByPlayer >= 0 && Main.player[taggedByPlayer] != null && Main.player[taggedByPlayer].active && !Main.player[taggedByPlayer].dead)
                {
                    distance = 600;
                }
                foreach (NPC target in Main.ActiveNPCs)
                {

                    if (target != npc && target.Distance(npc.Center) < distance)
                    {

                        for (int i = 0; i < BuffLoader.BuffCount; i++)
                        {
                            //what?
                            //if (HasDoTBuff(npc, i) >= 0 && HasDoTBuff(target, i) == -1)
                            //{
                            //    target.AddBuff(i, HasDoTBuff(npc, i));
                            //}
                        }
                    }
                }
            }*/
            if (npc.type == NPCID.KingSlime && BossRushEvent.BossRushActive && npc.GetLifePercent() <= 0.5f)
            {
                if (BossRushDialogueSystem.Phase < CalamityMod.Enums.BossRushDialoguePhase.TierOneComplete)
                    BossRushDialogueSystem.StartDialogue(CalamityMod.Enums.BossRushDialoguePhase.TierOneComplete);
                if (BossRushDialogueSystem.currentSequenceIndex == 2 && BossRushDialogueSystem.Phase == CalamityMod.Enums.BossRushDialoguePhase.TierOneComplete)
                {
                    Projectile.NewProjectileDirect(npc.GetSource_Death(), npc.Center + new Vector2(0, -800), new Vector2(0, 20), ModContent.ProjectileType<PenetratorThrown>(), 20000, 1, 0);
                    npc.StrikeInstantKill();
                }
            }
            return base.PreAI(npc);
        }
        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (WulfrumScanned >= 0 && HasAnyDoTDebuff(npc))
            {
                int DoTNormal = 35;
                int DoTForce = 100;
                Player owner = Main.player[Main.projectile[WulfrumScanned].owner];
                if (owner != null && owner.active && !owner.dead && owner.ForceEffect<WulfrumEffect>())
                {
                    npc.lifeRegen -= DoTForce;
                    if (damage < (int)(DoTForce / 10f))
                        damage = (int)(DoTForce / 10f);
                }
                else
                {
                    npc.lifeRegen -= DoTNormal;
                    if (damage < (int)(DoTNormal / 10f))
                        damage = (int)(DoTNormal / 10f);
                }

            }
            WulfrumScanned = -1;
        }
    }

}
