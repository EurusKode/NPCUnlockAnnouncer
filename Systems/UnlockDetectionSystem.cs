using Terraria;
using Terraria.ModLoader;
using NPCUnlockAnnouncer.CensusIntegration;

namespace NPCUnlockAnnouncer.Systems
{
    /// <summary>
    /// Periodically checks for newly unlocked NPCs using Census.
    /// </summary>
    public class UnlockDetectionSystem : ModSystem
    {
        // How often we check for new unlocks (in ticks)
        // 60 ticks = 1 second
        private const int CheckInterval = 300; // 5 seconds

        private int tickCounter;

        /// <summary>
        /// Called every game tick.
        /// </summary>
        public override void PostUpdateWorld()
        {
            // Do nothing if no world is loaded
            if (!Main.gameMenu)
            {
                tickCounter++;

                if (tickCounter >= CheckInterval)
                {
                    tickCounter = 0;

                    CensusUnlockProvider.CheckForNewUnlocks();
                }
            }
        }

        /// <summary>
        /// Reset counter when entering a world.
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
