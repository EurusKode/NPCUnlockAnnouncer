using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements; // 👈 ESTA LÍNEA CLAVE
using Terraria.UI;

namespace NPCUnlockAnnouncer.UI
{
    /// <summary>
    /// UI panel shown when an NPC is unlocked.
    /// </summary>
    public class NPCUnlockUI : UIState
    {
        private UIText npcNameText;
        private UIText loreText;
        private UIImage npcIcon;

        public override void OnInitialize()
        {
            // Main panel
            UIPanel panel = new UIPanel();
            panel.SetPadding(12);
            panel.Width.Set(420f, 0f);
            panel.Height.Set(120f, 0f);
            panel.HAlign = 0.5f;
            panel.VAlign = 0.1f;
            Append(panel);

            // NPC icon
            npcIcon = new UIImage(TextureAssets.Npc[0]);
            npcIcon.Left.Set(10f, 0f);
            npcIcon.Top.Set(10f, 0f);
            npcIcon.Width.Set(64f, 0f);
            npcIcon.Height.Set(64f, 0f);
            panel.Append(npcIcon);

            // NPC name
            npcNameText = new UIText("NPC Unlocked!");
            npcNameText.Left.Set(90f, 0f);
            npcNameText.Top.Set(10f, 0f);
            panel.Append(npcNameText);

            // Lore text
            loreText = new UIText("");
            loreText.Left.Set(90f, 0f);
            loreText.Top.Set(40f, 0f);
            loreText.Width.Set(-100f, 1f);
            panel.Append(loreText);
        }

        /// <summary>
        /// Updates the UI content.
        /// </summary>
        public void SetNPCInfo(int npcType, string npcName, string lore)
        {
            npcIcon.SetImage(TextureAssets.Npc[npcType]);
            npcNameText.SetText(npcName);
            loreText.SetText(lore);
        }
    }
}
