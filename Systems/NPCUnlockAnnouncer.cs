using Terraria;
using Terraria.ModLoader;
using NPCUnlockAnnouncer.CensusIntegration;

namespace NPCUnlockAnnouncer.Systems
{
    /// <summary>
    /// Periodically checks Census to detect newly unlocked NPCs.
    /// </summary>
    public class UnlockDetectionSystem : ModSystem
    {
        // 60 ticks = 1 second
        private const int CheckInterval = 300; // 5 seconds

        private int tickCounter;

        /// <summary>
        /// Runs after the world updates each tick.
        /// </summary>
        public override void PostUpdateWorld()
        {
            // Do not run logic while in the main menu
            if (Main.gameMenu)
                return;

            tickCounter++;

            if (tickCounter >= CheckInterval)
            {
                tickCounter = 0;

                // Ask Census provider to check for new unlocks
                CensusUnlockProvider.CheckForNewUnlocks();
            }
        }

        /// <summary>
        /// Reset counter when a world is loaded.
        /// </summary>
        public override void OnWorldLoad()
        {
            tickCounter = 0;
        }

        /// <summary>
        /// Reset counter when leaving a world.
        /// </summary>
        public override void OnWorldUnload()
        {
            tickCounter = 0;
        }
    }
}
