using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Terraria.ModLoader;

namespace NPCUnlockAnnouncer.Data
{
    /// <summary>
    /// Loads and provides lore data for NPCs from npc_lore.json.
    /// </summary>
    public static class LoreDatabase
    {
        private static Dictionary<string, LoreEntry> loreData;

        public static void Load(Mod mod)
        {
            loreData = new Dictionary<string, LoreEntry>();

            try
            {
                // Correct way to read files packaged inside a tModLoader mod
                using Stream stream = mod.GetFileStream("Data/npc_lore.json");
                using StreamReader reader = new StreamReader(stream);

                string json = reader.ReadToEnd();

                loreData = JsonSerializer.Deserialize<Dictionary<string, LoreEntry>>(json);
            }
            catch
            {
                // If the file does not exist or is malformed, fail safely
                loreData = new Dictionary<string, LoreEntry>();
            }
        }

        public static LoreEntry GetLore(string npcKey)
        {
            if (loreData != null && loreData.TryGetValue(npcKey, out var entry))
                return entry;

            return LoreEntry.Default;
        }
    }

    public class LoreEntry
    {
        public string title { get; set; }
        public string lore { get; set; }

        public static LoreEntry Default => new LoreEntry
        {
            title = "New NPC Unlocked",
            lore = "A new ally has joined your world."
        };
    }
}
