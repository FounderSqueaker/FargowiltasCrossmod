using Fargowiltas.Common.Configs;
using Fargowiltas.NPCs;
using Fargowiltas.Projectiles;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Core.Calamity.Systems;
using FargowiltasSouls.Content.Items;
using FargowiltasSouls.Content.NPCs;
using FargowiltasSouls.Core;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace FargowiltasSouls.Content.UI.Elements
{
    public class PriorityTogglerUI : UIPanel
    {
        public Texture2D Texture;
        public string Text;
        public bool SetState;
        public bool Hovering = false;

        public PriorityTogglerUI(Texture2D tex, string text, bool setState)
        {
            Texture = tex;
            Text = text;
            SetState = setState;

            Width.Set(24, 0);
            Height.Set(26, 0);
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime); // don't remove.

            bool conditions = Main.playerInventory; //&&  WorldSavingSystem.EternityMode;

            if (ContainsPoint(Main.MouseScreen) && conditions)
            {
                if (!Hovering)
                    SoundEngine.PlaySound(SoundID.MenuTick);
                Hovering = true;
                Main.LocalPlayer.mouseInterface = true;
            }
            else
                Hovering = false;


            if (conditions && ContainsPoint(Main.MouseScreen) && Main.mouseLeft && PlayerInput.MouseInfoOld.LeftButton == ButtonState.Released)
            {
                
                    
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            //if (!WorldSavingSystem.EternityMode)
            //    return;

            //base.DrawSelf(spriteBatch);

            CalculatedStyle style = GetDimensions();
            // Logic
            if (IsMouseHovering)
            {
                if (Main.mouseLeft && Main.mouseLeftRelease)
                {
                    if (!LumUtils.AnyBosses() && CalDLCConfig.Instance.EternityPriorityOverRev != SetState)
                    {
                        string mode = SetState ? "Eternity" : "Calamity";
                        FargoSoulsUtil.PrintLocalization($"Mods.FargowiltasCrossmod.UI.{mode}PriorityToggle", new Color(175, 75, 255));
                        CalDLCConfig.Instance.EternityPriorityOverRev = SetState;
                        CalDLCConfig.Instance.SaveChanges(); 
                    }
                }

                Vector2 textPosition = Main.MouseScreen + new Vector2(21, 21);
                string text = Text;

                Utils.DrawBorderString(
                    spriteBatch,
                    text,
                    textPosition,
                    Color.White);
            }
            bool active = CalDLCConfig.Instance.EternityPriorityOverRev == SetState;
            // Drawing
            Vector2 position = PriorityTogglerButton.DrawCenter - Vector2.UnitY * 20 * SetState.ToDirectionInt(); // style.Position();
            if (active)
            {
                for (int j = 0; j < 12; j++)
                {
                    Vector2 afterimageOffset = (MathHelper.TwoPi * j / 12f).ToRotationVector2() * 1f;
                    Color glowColor = Color.Silver;
                    spriteBatch.Draw(Texture, position + new Vector2(2) + afterimageOffset, Texture.Bounds, glowColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
                }
            }
            spriteBatch.Draw(Texture, position + new Vector2(2), Texture.Bounds, Color.White * (active ? 1f : 0.7f), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);

        }
    }
}
