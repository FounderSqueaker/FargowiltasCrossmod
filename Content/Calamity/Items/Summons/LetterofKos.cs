using CalamityMod.Items.SummonItems;
using CalamityMod.NPCs.Signus;
using Fargowiltas.Items.Summons;
using FargowiltasCrossmod.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Items.Summons
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class LetterofKos : BaseSummon
    {
        public override int NPCType => ModContent.NPCType<Signus>();
        public override void AddRecipes()
        {
            Recipe.Create(Type).AddIngredient<MarkofProvidence>().AddTile(TileID.WorkBenches).Register();
            Recipe.Create(ModContent.ItemType<MarkofProvidence>()).AddIngredient(Type).AddTile(TileID.WorkBenches).Register();
        }
    }
}
