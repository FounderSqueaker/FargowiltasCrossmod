using CalamityMod;
using CalamityMod.Events;
using CalamityMod.NPCs;
using CalamityMod.Skies;
using Fargowiltas.Content.NPCs;
using CalamityMod.Systems;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Core.Common;
using FargowiltasSouls.Core.Systems;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Core.Calamity.Systems
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class CalDLCWorldUpdatingSystem : ModSystem
    {
        public static bool InfernumStateLastFrame = false;
        public override void PreUpdateWorld()
        {
            if (ModCompatibility.Calamity.Loaded)
            {
                ModCompatibility.SoulsMod.Mod.Call("EternityVanillaBossBehaviour", CalDLCConfig.Instance.EternityPriorityOverRev);
                if (CalDLCConfig.Instance.EternityPriorityOverRev && WorldSavingSystem.EternityMode)
                {
                    CalamityVanillaAIOverrideNPC.Enabled = false;
                }
                else
                {
                    CalamityVanillaAIOverrideNPC.Enabled = true;
                }
            }

        }
        [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
        public override void PostUpdateWorld()
        {
            if (ModCompatibility.Calamity.Loaded)
            {
                if (!Main.expertMode && (CalDLCWorldSavingSystem.EternityRev || CalDLCWorldSavingSystem.MasoDeath))
                {
                    CalDLCWorldSavingSystem.EternityRev = false;
                    CalDLCWorldSavingSystem.MasoDeath = false;
                    if (Main.netMode != NetmodeID.SinglePlayer)
                        PacketManager.SendPacket<EternityCalPacket>();
                }
                if (WorldSavingSystem.EternityMode && !CalDLCWorldSavingSystem.EternityRev)
                {
                    CalDLCWorldSavingSystem.EternityRev = true;
                    if (Main.netMode != NetmodeID.SinglePlayer)
                        PacketManager.SendPacket<EternityCalPacket>();
                }
                bool infernum = false;
                if (ModCompatibility.InfernumMode.Loaded)
                    if (ModCompatibility.InfernumMode.InfernumDifficulty && CalDLCConfig.Instance.InfernumDisablesEternity)
                        infernum = true;
                if (CalDLCWorldSavingSystem.EternityRev && !WorldSavingSystem.EternityMode && !infernum && Main.expertMode)
                {
                    
                    WorldSavingSystem.ShouldBeEternityMode = true;
                    WorldSavingSystem.EternityMode = true;
                    if (Main.netMode != NetmodeID.SinglePlayer)
                        PacketManager.SendPacket<EternityCalPacket>();
                }
                if (WorldSavingSystem.EternityMode && !WorldSavingSystem.SpawnedDevi && DLCUtils.HostCheck)
                {
                    int devi = NPC.NewNPC(new EntitySource_SpawnNPC(), Main.spawnTileX * 16, Main.spawnTileY * 16 - 400, ModContent.NPCType<Deviantt>());
                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, devi);
                    WorldSavingSystem.SpawnedDevi = true;
                }
                //DownedBossSystem.startedBossRushAtLeastOnce = false;
                if (BossRushEvent.BossRushActive)
                {
                    
                }
                if ((Main.IsItRaining || Main.IsItStorming) && BossRushEvent.BossRushActive)
                {
                    Main.StopRain();
                    Main.cloudAlpha = 0;
                    if (Main.netMode == NetmodeID.Server)
                        Main.SyncRain();
                }
            }
            
            if (ModCompatibility.InfernumMode.Loaded)
            {
                if (ModCompatibility.InfernumMode.InfernumDifficulty && !InfernumStateLastFrame)
                {
                    //CalDLCConfig.Instance.EternityPriorityOverRev = false;


                    if (CalDLCConfig.Instance.InfernumDisablesEternity)
                    {
                        WorldSavingSystem.EternityMode = false;
                        WorldSavingSystem.ShouldBeEternityMode = false;
                        Main.NewText(Language.GetTextValue("Mods.FargowiltasCrossmod.EmodeBannedByInfernum"));
                        if (Main.netMode != NetmodeID.SinglePlayer)
                            PacketManager.SendPacket<EternityCalPacket>();
                    }
                    else
                    {
                        //Main.NewText("[c/9c0000:Infernum Mode] detected. [c/00ffee:Eternity Priority over Calamity Bosses] has been disabled to prevent bugs.\n" +
                            //"[c/00ffee:Eternity Priority over Calamity Bosses] can be re-enabled in the config, but things will break.");
                    }
                }
                if (ModCompatibility.InfernumMode.InfernumDifficulty) InfernumStateLastFrame = true;
                else InfernumStateLastFrame = false;
            }
            
            base.PostUpdateWorld();
        }
    }
}
