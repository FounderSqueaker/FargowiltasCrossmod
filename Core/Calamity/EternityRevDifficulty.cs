using CalamityMod.Systems;
using CalamityMod.World;
using Fargowiltas.Projectiles;
using FargowiltasCrossmod.Core.Calamity.Systems;
using FargowiltasSouls.Content.NPCs;
using FargowiltasSouls.Core.Systems;
using FargowiltasSouls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static CalamityMod.Systems.DifficultyModeSystem;
using Terraria.Audio;
using System.Collections.Generic;

namespace FargowiltasCrossmod.Core.Calamity
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class EternityRevDifficulty : DifficultyMode
    {
        public override LocalizedText Name => Language.GetText("Mods.FargowiltasCrossmod.EternityRevDifficulty.Name");
        public override LocalizedText FTWName => Language.GetText("Mods.FargowiltasCrossmod.EternityRevDifficulty.Name");
        public override Color ChatTextColor => Color.Cyan;
        public override Color FTWTextColor => Color.Cyan;
        public override SoundStyle ActivationSound => SoundID.Roar with { Pitch = -0.5f };
        public override int BackBoneGameModeID => GameModeID.Expert;
        public override LocalizedText ShortDescription => Language.GetText("Mods.FargowiltasCrossmod.EternityRevDifficulty.ShortDescription");
        public override Asset<Texture2D> TextureDisabled
        {
            get
            {
                _textureDisabled ??= ModContent.Request<Texture2D>("FargowiltasCrossmod/Assets/EternityRevIcon");

                return _textureDisabled;
            }
        }
        public override float DifficultyScale => 2;
        public override bool Enabled
        {
            get => CalDLCWorldSavingSystem.EternityRev;
            set
            {
                CalDLCWorldSavingSystem.EternityRev = value;
                if (value)
                {
                    CalamityWorld.revenge = true;
                    CalamityWorld.death = false;

                    int deviType = ModContent.NPCType<UnconsciousDeviantt>();
                    if (!WorldSavingSystem.SpawnedDevi && !NPC.AnyNPCs(deviType))
                    {
                        WorldSavingSystem.SpawnedDevi = true;

                        Vector2 spawnPos = (Main.zenithWorld || Main.remixWorld) ? Main.LocalPlayer.Center : Main.LocalPlayer.Center - 1000 * Vector2.UnitY;
                        Projectile.NewProjectile(Main.LocalPlayer.GetSource_Misc(""), spawnPos, Vector2.Zero, ModContent.ProjectileType<SpawnProj>(), 0, 0, Main.myPlayer, deviType);

                        FargoSoulsUtil.PrintLocalization("Announcement.HasAwoken", new Color(175, 75, 255), Language.GetTextValue("Mods.Fargowiltas.NPCs.Deviantt.DisplayName"));
                    }
                }
                bool emode = value;
                if (ModCompatibility.InfernumMode.Loaded)
                    if (ModCompatibility.InfernumMode.InfernumDifficulty && CalDLCConfig.Instance.InfernumDisablesEternity)
                        emode = false;

                if (Main.expertMode)
                {
                    WorldSavingSystem.EternityMode = emode;
                    WorldSavingSystem.ShouldBeEternityMode = emode;
                }
                if (Main.netMode != NetmodeID.SinglePlayer)
                    PacketManager.SendPacket<EternityCalPacket>();
            }
        }

        private Asset<Texture2D> _texture;
        public override Asset<Texture2D> Texture
        {
            get
            {
                _texture ??= ModContent.Request<Texture2D>("FargowiltasCrossmod/Assets/EternityRevIcon");

                return _texture;
            }
        }

        //TODO: add conditions to this description, for priority and Maso line
        public override LocalizedText ExpandedDescription => Language.GetText("Mods.FargowiltasCrossmod.EternityRevDifficulty.ExpandedDescription");

        public override int[] FavoredDifficultyAtTier(int tier)
        {
            DifficultyMode[] tierList = DifficultyTiers[tier];
            List<int> list = new List<int>();
            for (int i = 0; i < tierList.Length; i++)
            {
                if (tierList[i].Name.Value == "Revengeance")
                    list.Add(i);
            }
            if (list.Count <= 0) list.Add(0);
            return list.ToArray();
        }
    }
}