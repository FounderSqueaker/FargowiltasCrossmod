using CalamityMod;
using CalamityMod.CalPlayer;
using FargowiltasSouls;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.ModPlayers;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Core.Calamity.Globals
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class CalDLCAddonGlobalItem : GlobalItem
    {
        public override void OnConsumeItem(Item item, Player player)
        {
            FargoSoulsPlayer fargoPlayer = player.FargoSouls();
            CalamityPlayer calPlayer = player.Calamity();

            //give hallow its proper heal value, additive with bloom stone's 50% mult
            if (player.HasEffect<HallowEffect>() && calPlayer.bloomStone)
            {
                //hallow and bloom stone must be disabled before fetching
                int hallowIndex = ModContent.GetInstance<HallowEffect>().Index;
                player.AccessoryEffects().ActiveEffects[hallowIndex] = calPlayer.bloomStone = false;

                float addedmult = (fargoPlayer.ForceEffect<HallowEnchant>() ? 1.7f : 1.4f) + 0.5f;
                fargoPlayer.HallowHealTotal = player.GetHealLife(item) * addedmult;
                fargoPlayer.HallowHealTime = 600;

                player.AccessoryEffects().ActiveEffects[hallowIndex] = calPlayer.bloomStone = true;
            }
        }

        public override bool? UseItem(Item item, Player player)
        {
            CalamityPlayer calPlayer = player.Calamity();

            //disable bloom stone's effects to transfer to hallow
            if (calPlayer.bloomStone)
            {
                calPlayer.bloomStoneTotalHeal = calPlayer.bloomStoneHealPool = 0;
            }

            return base.UseItem(item, player);
        }
        public override void GetHealLife(Item item, Player player, bool quickHeal, ref int healValue)
        {

        }
    }
}
