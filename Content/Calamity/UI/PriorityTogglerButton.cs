using CalamityMod.UI.ModeIndicator;
using FargowiltasSouls.Content.UI.Elements;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace FargowiltasSouls.Content.UI
{
    public class PriorityTogglerButton : FargoUI
    {
        public override int InterfaceIndex(List<GameInterfaceLayer> layers, int vanillaInventoryIndex) => vanillaInventoryIndex;
        public override string InterfaceLayerName => "DLC: Priority Toggler";
        public PriorityTogglerUI EmodePriority;
        public PriorityTogglerUI CalamityPriority;
        public override void OnLoad()
        {
            FargoUIManager.Open<PriorityTogglerButton>();
        }
        public override void UpdateUI()
        {
            if (!Main.playerInventory)
                FargoUIManager.Close<PriorityTogglerButton>();
            else
                FargoUIManager.Open<PriorityTogglerButton>();
        }
        public static Vector2 DrawCenter => ModeIndicatorUI.DrawCenter + Vector2.UnitX * 70;
        public override void OnActivate()
        {
            int vOffset = 20;
            EmodePriority = new PriorityTogglerUI(ModContent.Request<Texture2D>("FargowiltasCrossmod/Content/Calamity/UI/EternityPriority", AssetRequestMode.ImmediateLoad).Value,
                Language.GetTextValue("Mods.FargowiltasCrossmod.UI.EternityPriority"),
                true
                );
            EmodePriority.Left.Set(DrawCenter.X, 0);
            EmodePriority.Top.Set(DrawCenter.Y - vOffset, 0);
            Append(EmodePriority);

            CalamityPriority = new PriorityTogglerUI(ModContent.Request<Texture2D>("FargowiltasCrossmod/Content/Calamity/UI/CalamityPriority", AssetRequestMode.ImmediateLoad).Value,
                Language.GetTextValue("Mods.FargowiltasCrossmod.UI.CalamityPriority"),
                false
                );
            CalamityPriority.Left.Set(DrawCenter.X, 0);
            CalamityPriority.Top.Set(DrawCenter.Y + vOffset, 0);
            Append(CalamityPriority);

            base.OnActivate();
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Main.playerInventory)
            {
                EmodePriority.Draw(spriteBatch);
                CalamityPriority.Draw(spriteBatch);
            }

        }
    }
}
