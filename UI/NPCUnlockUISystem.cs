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
        private NPCUnlockUI unlockUI;

        private int displayTimer;
        private const int DisplayTime = 300; // 5 seconds

        public override void Load()
        {
            if (Main.dedServ)
                return;

            Instance = this;

            unlockUI = new NPCUnlockUI();
            unlockUI.Activate();

            userInterface = new UserInterface();
            userInterface.SetState(null);
        }

        public override void UpdateUI(GameTime gameTime)
        {
            if (displayTimer > 0)
            {
                displayTimer--;
                userInterface?.Update(gameTime);
            }
        }

        public override void ModifyInterfaceLayers(System.Collections.Generic.List<GameInterfaceLayer> layers)
        {
            if (displayTimer <= 0 || userInterface?.CurrentState == null)
                return;

            int index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (index != -1)
            {
                layers.Insert(index, new LegacyGameInterfaceLayer(
                    "NPCUnlockAnnouncer: NPC Unlock UI",
                    delegate
                    {
                        userInterface.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI
                ));
            }
        }

        /// <summary>
        /// Shows the unlock UI for a specific NPC.
        /// </summary>
        public void ShowNPCUnlock(int npcType, string title, string lore)
        {
            unlockUI.SetNPCInfo(npcType, title, lore);
            userInterface.SetState(unlockUI);
            displayTimer = DisplayTime;
        }
    }
}
