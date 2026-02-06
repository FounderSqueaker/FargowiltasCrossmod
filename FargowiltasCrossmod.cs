global using LumUtils = Luminance.Common.Utilities.Utilities;
using CalamityMod;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Skies;
using CalamityMod.Systems.Collections;
using FargowiltasCrossmod.Content.Calamity.Bosses.Cryogen;
using FargowiltasCrossmod.Content.Calamity.Bosses.HiveMind;
using FargowiltasCrossmod.Content.Common.Bosses.Mutant;
using FargowiltasCrossmod.Content.Common.Sky;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity.Systems;
using FargowiltasCrossmod.Core.Common.Globals;
using FargowiltasSouls;
using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Core.Toggler;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;

namespace FargowiltasCrossmod;

public class FargowiltasCrossmod : Mod
{
    internal static FargowiltasCrossmod Instance;
    public static bool EnchantLoadingEnabled = false;
    public override void Load()
    {
        Instance = this;

        ModCompatibility.BossChecklist.AdjustValues();
    }
    public override void Unload()
    {
        Instance = null;
    }

    /* no need for this anymore
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public static void LoadTogglesFromType(Type type)
    {

        ToggleCollection toggles = (ToggleCollection)Activator.CreateInstance(type);

        if (toggles.Active)
        {
            ModContent.GetInstance<FargowiltasCrossmod>().Logger.Info($"ToggleCollection found: {nameof(type)}");
            List<Toggle> toggleCollectionChildren = toggles.Load();
            foreach (Toggle toggle in toggleCollectionChildren)
            {
                ToggleLoader.RegisterToggle(toggle);
            }
        }
    }
    */
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public override void PostSetupContent()
    {
        if (ModCompatibility.Calamity.Loaded)
        {
            PostSetupContent_Calamity();
            SkyManager.Instance["FargowiltasCrossmod:Permafrost"] = new PermafrostSky();
            SkyManager.Instance["FargowiltasCrossmod:HiveMind"] = new HiveMindSky();
        }

        if (MutantDLC.ShouldDoDLC)
        {
            SkyManager.Instance["FargowiltasSouls:MutantBoss"] = new MutantDLCSky();
        }

        #region difficultyWorldScreen

        //doesnt work no idea why
        //Func<WorldFileData, bool> erevEnabled = data =>
        //{
        //    if (!data.TryGetHeaderData<CalDLCWorldSavingSystem>(out var tagData))
        //        return false;
        //    return tagData.GetList<string>("downed").Contains("EternityRevActive");
        //};
        //Func<WorldFileData, bool> masodeathEnabled = data =>
        //{
        //    if (!data.TryGetHeaderData<CalDLCWorldSavingSystem>(out var tagData))
        //        return false;
        //    return tagData.GetList<string>("downed").Contains("MasoDeathActive");
        //};
        //Mod luminance = ModLoader.GetMod("Luminance");
        //luminance.Call("RegisterWorldInfoIcon", "FargowiltasCrossmod/Assets/EternityRevIcon", "Mods.FargowiltasCrossmod.EternityRevDifficulty.Name", erevEnabled, (byte)60);
        //luminance.Call("RegisterWorldInfoIcon", "FargowiltasCrossmod/Assets/MasoDeathIcon", "Mods.FargowiltasCrossmod.MasoDeathDifficulty.Name", masodeathEnabled, (byte)60);

        #endregion
    }
    //[JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    //public DamageClass rogueDamageClass => ModContent.GetInstance<RogueDamageClass>();
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public void PostSetupContent_Calamity()
    {
        
        /* doesn't seem to be working, may investigate later
        List<int> CalamityReworkedSpears = new List<int>
        {
            ModContent.ItemType<AstralPike>()
        };
        SpearRework.ReworkedSpears.AddRange(CalamityReworkedSpears);
        */
        
        /*
         * PR'd to Calamity
        #region Stat Sheet
        double Damage(DamageClass damageClass) => Math.Round(Main.LocalPlayer.GetTotalDamage(damageClass).Additive * Main.LocalPlayer.GetTotalDamage(damageClass).Multiplicative * 100 - 100);
        int Crit(DamageClass damageClass) => (int)Main.LocalPlayer.GetTotalCritChance(damageClass);

        int rogueItem = ModContent.ItemType<WulfrumKnife>();
        Func<string> rogueDamage = () => $"Rogue Damage: {Damage(rogueDamageClass)}%";
        Func<string> rogueCrit = () => $"Rogue Critical: {Crit(rogueDamageClass)}%";
        ModCompatibility.MutantMod.Mod.Call("AddStat", rogueItem, rogueDamage);
        ModCompatibility.MutantMod.Mod.Call("AddStat", rogueItem, rogueCrit);
        #endregion
        */
    }
    public override void HandlePacket(BinaryReader reader, int whoAmI) => PacketManager.ReceivePacket(reader);
}