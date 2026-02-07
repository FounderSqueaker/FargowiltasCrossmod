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
    public class MasoDeathDifficulty : DifficultyMode
    {
        public override Asset<Texture2D> OutlineTexture
        {
            get
            {
                _outlineTexture ??= ModContent.Request<Texture2D>("FargowiltasCrossmod/Assets/MasoDeathIcon_Outline");

                return _outlineTexture;
            }
        }
        public override LocalizedText Name => Language.GetText("Mods.FargowiltasCrossmod.MasoDeathDifficulty.Name");
        public override LocalizedText FTWName => Language.GetText("Mods.FargowiltasCrossmod.MasoDeathDifficulty.FTWName");
        public override Color ChatTextColor => Color.DarkRed;
        public override Color? FTWTextColor => Color.DarkRed;
        public override SoundStyle ActivationSound => SoundID.Roar with { Pitch = -0.5f };
        public override int BackBoneGameModeID => GameModeID.Master;
        public override LocalizedText ShortDescription => Language.GetText("Mods.FargowiltasCrossmod.MasoDeathDifficulty.ShortDescription");
        public override Asset<Texture2D> TextureDisabled {
            get
            {
                _textureDisabled ??= ModContent.Request<Texture2D>("FargowiltasCrossmod/Assets/MasoDeathIcon_Off");

                return _textureDisabled;
            }
        }
        public override float DifficultyScale => 0.25f;
        public override bool Enabled
        {
            get => CalDLCWorldSavingSystem.MasoDeath;
            set
            {
                CalDLCWorldSavingSystem.EternityRev = value;
                CalDLCWorldSavingSystem.MasoDeath = value;
                if (value)
                {
                    CalamityWorld.revenge = true;
                    CalamityWorld.death = true;

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
                WorldSavingSystem.EternityMode = emode;
                WorldSavingSystem.ShouldBeEternityMode = emode;
                //if (value)
                //{
                //    Main.GameMode = GameModeID.Master;
                //}
                
                if (Main.netMode != NetmodeID.SinglePlayer)
                    PacketManager.SendPacket<EternityCalPacket>();
            }
        }

        public override Asset<Texture2D> Texture
        {
            get
            {
                _texture ??= ModContent.Request<Texture2D>("FargowiltasCrossmod/Assets/MasoDeathIcon");

                return _texture;
            }
        }

        //TODO: add conditions to this description, for priority and Maso line
        public override LocalizedText ExpandedDescription => Language.GetText("Mods.FargowiltasCrossmod.MasoDeathDifficulty.ExpandedDescription");
        public override int[] FavoredDifficultyAtTier(int tier)
        {
            DifficultyMode[] tierList = DifficultyTiers[tier];
            List<int> list = new List<int>();
            for (int i = 0; i < tierList.Length; i++)
            {
                if (tierList[i] is DeathDifficulty)
                    list.Add(i);
            }
            if (list.Count <= 0) list.Add(0);
            return list.ToArray();
        }
        public override bool IsBasedOn(DifficultyMode mode)
        {
            return mode is DeathDifficulty;
        }
    }
}