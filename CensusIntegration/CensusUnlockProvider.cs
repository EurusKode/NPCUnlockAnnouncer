using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using NPCUnlockAnnouncer.Systems;

namespace NPCUnlockAnnouncer.CensusIntegration
{
    /// <summary>
    /// Uses Census to detect when Town NPCs are officially unlocked.
    /// Returns the NPC key when a new unlock is detected.
    /// </summary>
    public static class CensusUnlockProvider
    {
        /// <summary>
        /// Checks for newly unlocked NPCs.
        /// Returns the npcKey (ModName/NPCName) if one is unlocked, otherwise null.
        /// </summary>
        public static string CheckForNewUnlocks()
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

                // Skip if already handled
                if (WorldUnlockData.IsNPCUnlocked(npcKey))
                    continue;

                // Ask Census
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
                    return npcKey; // 👈 CLAVE: devolvemos el NPC desbloqueado
                }
            }

            return null;
        }

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
