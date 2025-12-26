using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using NPCUnlockAnnouncer.Systems;

namespace NPCUnlockAnnouncer.CensusIntegration
{
    /// <summary>
    /// Uses Census to detect when Town NPCs are officially unlocked.
    /// </summary>
    public static class CensusUnlockProvider
    {
        public static void CheckForNewUnlocks()
        {
            // Census is a required dependency, but we still guard against null
            Mod census = ModLoader.GetMod("Census");
            if (census == null)
                return;

            // Iterate through all NPC types (vanilla + modded)
            for (int npcType = 0; npcType < NPCLoader.NPCCount; npcType++)
            {
                // Get default NPC instance (safe sample NPC)
                NPC npc = ContentSamples.NpcsByNetId[npcType];

                // Only Town NPCs can be unlocked
                if (!npc.townNPC)
                    continue;

                // Build NPC key: ModName/NPCName
                string npcKey = GetNPCKey(npcType);

                // Skip if already announced
                if (WorldUnlockData.IsNPCUnlocked(npcKey))
                    continue;

                // Ask Census if the NPC is unlocked
                object result = census.Call("TownNPCUnlocked", npcType);

                bool unlocked = false;

                // Handle different possible return types from Census
                if (result is bool boolResult)
                {
                    unlocked = boolResult;
                }
                else if (result is string stringResult)
                {
                    // Census often returns strings like "Unlocked"
                    unlocked = stringResult.Equals("Unlocked", StringComparison.OrdinalIgnoreCase);
                }

                if (unlocked)
                {
                    WorldUnlockData.MarkNPCUnlocked(npcKey);

                    // UI / sound triggering is handled elsewhere
                }
            }
        }

        /// <summary>
        /// Builds a unique NPC key in the format ModName/NPCName.
        /// </summary>
        private static string GetNPCKey(int npcType)
        {
            // Modded NPC
            ModNPC modNpc = ModContent.GetModNPC(npcType);
            if (modNpc != null)
            {
                return $"{modNpc.Mod.Name}/{modNpc.Name}";
            }

            // Vanilla NPC
            return $"Terraria/{NPCID.Search.GetName(npcType)}";
        }
    }
}
