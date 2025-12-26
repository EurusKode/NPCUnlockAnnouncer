using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using NPCUnlockAnnouncer.Systems;

namespace NPCUnlockAnnouncer.CensusIntegration
{
    /// <summary>
    /// Uses Census to detect when Town NPCs are officially unlocked.
    /// Returns the npcType and npcKey when a new unlock is detected.
    /// </summary>
    public static class CensusUnlockProvider
    {
        /// <summary>
        /// Checks for newly unlocked NPCs.
        /// Returns (npcType, npcKey) if one is unlocked, otherwise null.
        /// </summary>
        public static (int npcType, string npcKey)? CheckForNewUnlocks()
        {
            Mod census = ModLoader.GetMod("Census");
            if (census == null)
                return null;

            for (int npcType = 0; npcType < NPCLoader.NPCCount; npcType++)
            {
                NPC npc = ContentSamples.NpcsByNetId[npcType];

                // Only Town NPCs
                if (!npc.townNPC)
                    continue;

                string npcKey = GetNPCKey(npcType);

                // Skip if already announced
                if (WorldUnlockData.IsNPCUnlocked(npcKey))
                    continue;

                // Ask Census about unlock state
                object result = census.Call("TownNPCUnlocked", npcType);

                bool unlocked = false;

                if (result is bool boolResult)
                {
                    unlocked = boolResult;
                }
                else if (result is string stringResult)
                {
                    unlocked = stringResult.Equals("Unlocked", StringComparison.OrdinalIgnoreCase);
                }

                if (unlocked)
                {
                    WorldUnlockData.MarkNPCUnlocked(npcKey);

                    return (npcType, npcKey);
                }
            }

            return null;
        }

        /// <summary>
        /// Builds a unique NPC key in the format ModName/NPCName.
        /// </summary>
        private static string GetNPCKey(int npcType)
        {
            ModNPC modNpc = ModContent.GetModNPC(npcType);
            if (modNpc != null)
            {
                return $"{modNpc.Mod.Name}/{modNpc.Name}";
            }

            return $"Terraria/{NPCID.Search.GetName(npcType)}";
        }
    }
}
