using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace NPCUnlockAnnouncer.UI
{
    public class NPCUnlockUISystem : ModSystem
    {
        public static NPCUnlockUISystem Instance;

        private UserInterface userInterface;
        private NPCUnlockUI ui;
        private NPCUnlockAnimator animator;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            Instance = this;

            ui = new NPCUnlockUI();
            ui.Activate();

            animator = new NPCUnlockAnimator();

            userInterface = new UserInterface();
        }

        public override void UpdateUI(GameTime gameTime)
        {
            if (!animator.IsVisible)
                return;

            animator.Update();
            ui.Banner.Top.Set(animator.CurrentY, 0f);
            userInterface.Update(gameTime);
        }

        public override void ModifyInterfaceLayers(System.Collections.Generic.List<GameInterfaceLayer> layers)
        {
            if (!animator.IsVisible)
                return;

            int index = layers.FindIndex(l => l.Name.Equals("Vanilla: Mouse Text"));
            if (index != -1)
            {
                layers.Insert(index, new LegacyGameInterfaceLayer(
                    "NPCUnlockAnnouncer: Animated Banner",
                    () =>
                    {
                        userInterface.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI
                ));
            }
        }

        public void ShowNPCUnlock(int npcType, string title, string subtitle)
        {
            ui.SetContent(npcType, title, subtitle);
            userInterface.SetState(ui);
            animator.Start();
        }
    }
}
