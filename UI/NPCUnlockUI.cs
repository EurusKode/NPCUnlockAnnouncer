using Microsoft.Xna.Framework;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace NPCUnlockAnnouncer.UI
{
    public class NPCUnlockUI : UIState
    {
        public UIPanel Banner;
        private UIImage npcIcon;
        private UIText titleText;
        private UIText subtitleText;

        public override void OnInitialize()
        {
            Banner = new UIPanel();
            Banner.SetPadding(10);
            Banner.Width.Set(420f, 0f);
            Banner.Height.Set(64f, 0f);
            Banner.HAlign = 0.5f;
            Append(Banner);

            npcIcon = new UIImage(TextureAssets.Npc[0]);
            npcIcon.Left.Set(8f, 0f);
            npcIcon.Top.Set(8f, 0f);
            npcIcon.Width.Set(48f, 0f);
            npcIcon.Height.Set(48f, 0f);
            Banner.Append(npcIcon);

            titleText = new UIText("NPC UNLOCKED", 1.3f);
            titleText.Left.Set(64f, 0f);
            titleText.Top.Set(4f, 0f);
            titleText.TextColor = Color.Gold;
            Banner.Append(titleText);

            subtitleText = new UIText("", 0.9f);
            subtitleText.Left.Set(64f, 0f);
            subtitleText.Top.Set(32f, 0f);
            subtitleText.TextColor = Color.LightGray;
            Banner.Append(subtitleText);
        }

        public void SetContent(int npcType, string title, string subtitle)
        {
            npcIcon.SetImage(TextureAssets.Npc[npcType]);
            titleText.SetText(title.ToUpper());
            subtitleText.SetText(subtitle);
        }
    }
}
