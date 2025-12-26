using Terraria;
using Terraria.ModLoader;
using NPCUnlockAnnouncer.CensusIntegration;
using NPCUnlockAnnouncer.UI;
using NPCUnlockAnnouncer.Data;
using Terraria.ID;

namespace NPCUnlockAnnouncer.Systems
{
    /// <summary>
    /// Periodically checks for newly unlocked NPCs using Census
    /// and triggers the UI notification.
    /// </summary>
    public class UnlockDetectionSystem : ModSystem
    {
        // 60 ticks = 1 second
        private const int CheckInterval = 300; // 5 seconds

        private int tickCounter;

        public override void PostUpdateWorld()
        {
            // Do nothing while in main menu
            if (Main.gameMenu)
                return;

            tickCounter++;

            if (tickCounter >= CheckInterval)
            {
                tickCounter = 0;

                // Check Census for a newly unlocked NPC
                var unlockResult = CensusUnlockProvider.CheckForNewUnlocks();

                if (unlockResult.HasValue)
                {
                    int npcType = unlockResult.Value.npcType;
                    string npcKey = unlockResult.Value.npcKey;

                    // Get lore (or fallback)
                    LoreEntry lore = LoreDatabase.GetLore(npcKey);

                // UI must only be shown on the client
                if (Main.netMode != NetmodeID.Server)
                {
                    NPCUnlockUISystem.Instance?.ShowNPCUnlock(
                        npcType,
                        lore.title,
                        lore.lore
                    );
                }

                }
            }
        }

        public override void OnWorldLoad()
        {
            tickCounter = 0;
        }

        public override void OnWorldUnload()
        {
            tickCounter = 0;
        }
    }
}
