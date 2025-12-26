using System.Collections.Generic;
using System.Linq;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;


namespace NPCUnlockAnnouncer.Systems
{
    /// <summary>
    /// This system stores which NPCs have already been announced
    /// as unlocked in the current world.
    /// 
    /// Data is saved per world, not per player.
    /// </summary>
    public class WorldUnlockData : ModSystem
    {
        /// <summary>
        /// Stores NPC identifiers in the format:
        /// "ModName/NPCName"
        /// </summary>
        public static HashSet<string> UnlockedNPCs = new();

        /// <summary>
        /// Called when a world is loaded.
        /// Initializes or clears the stored data.
        /// </summary>
        public override void OnWorldLoad()
        {
            UnlockedNPCs.Clear();
        }

        /// <summary>
        /// Called when a world is unloaded.
        /// Ensures no data leaks between worlds.
        /// </summary>
        public override void OnWorldUnload()
        {
            UnlockedNPCs.Clear();
        }

        /// <summary>
        /// Saves the unlocked NPC data to the world file.
        /// </summary>
        public override void SaveWorldData(TagCompound tag)
        {
            tag["UnlockedNPCs"] = new List<string>(UnlockedNPCs);
        }

        /// <summary>
        /// Loads the unlocked NPC data from the world file.
        /// </summary>
        public override void LoadWorldData(TagCompound tag)
        {
            if (tag.ContainsKey("UnlockedNPCs"))
            {
                UnlockedNPCs = tag.Get<List<string>>("UnlockedNPCs").ToHashSet();
            }
        }

        /// <summary>
        /// Checks if an NPC has already been announced.
        /// </summary>
        public static bool IsNPCUnlocked(string npcKey)
        {
            return UnlockedNPCs.Contains(npcKey);
        }

        /// <summary>
        /// Marks an NPC as announced so it will not trigger again.
        /// </summary>
        public static void MarkNPCUnlocked(string npcKey)
        {
            UnlockedNPCs.Add(npcKey);
        }
    }
}
