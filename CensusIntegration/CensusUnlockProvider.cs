using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using NPCUnlockAnnouncer.Systems;

namespace NPCUnlockAnnouncer.CensusIntegration
{
    public static class CensusUnlockProvider
    {
        public static (int npcType, string npcKey)? CheckForNewUnlocks()
        {
            Mod census = ModLoader.GetMod("Census");
            if (census == null)
                return null;

            for (int npcType = 0; npcType < NPCLoader.NPCCount; npcType++)
            {
                NPC npc = ContentSamples.NpcsByNetId[npcType];

                if (!npc.townNPC)
                    continue;

                string npcKey = GetNPCKey(npcType);

                if (WorldUnlockData.IsNPCUnlocked(npcKey))
                    continue;

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
