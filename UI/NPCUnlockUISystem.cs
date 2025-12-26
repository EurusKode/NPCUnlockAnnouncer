using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using NPCUnlockAnnouncer.Systems;

namespace NPCUnlockAnnouncer.UI
{
    /// <summary>
    /// Manages when the NPC unlock UI is shown.
    /// </summary>
    public class NPCUnlockUISystem : ModSystem
    {
        private UserInterface userInterface;
        private NPCUnlockUI unlockUI;

        private int displayTimer;
        private const int DisplayTime = 300; // 5 seconds

        public override void Load()
        {
            if (!Main.dedServ)
            {
                unlockUI = new NPCUnlockUI();
                unlockUI.Activate();

                userInterface = new UserInterface();
            }
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
            int index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));

            if (index != -1 && displayTimer > 0)
            {
                layers.Insert(index, new LegacyGameInterfaceLayer(
                    "NPCUnlockAnnouncer: NPC Unlock UI",
                    delegate
                    {
                        userInterface.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }

        /// <summary>
        /// Shows the unlock UI for a specific NPC.
        /// </summary>
        public void ShowNPCUnlock(int npcType, string npcName, string lore)
        {
            unlockUI.SetNPCInfo(npcType, npcName, lore);
            userInterface.SetState(unlockUI);
            displayTimer = DisplayTime;
        }
    }
}
