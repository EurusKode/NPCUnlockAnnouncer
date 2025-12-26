using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace NPCUnlockAnnouncer.Data
{
    public static class LoreDatabase
    {
        private static Dictionary<string, LoreEntry> loreData;

        public static void Load(Mod mod)
        {
            loreData = new Dictionary<string, LoreEntry>();
            try
            {
                using Stream stream = mod.GetFileStream("Data/npc_lore.json");
                using StreamReader reader = new StreamReader(stream);
                string json = reader.ReadToEnd();
                loreData = JsonSerializer.Deserialize<Dictionary<string, LoreEntry>>(json);
            }
            catch
            {
                loreData = new Dictionary<string, LoreEntry>();
            }
        }

        // --- NUEVO: Método para buscar por ID numérico ---
        public static LoreEntry GetLore(int npcId)
        {
            string key = GetNPCKey(npcId); // Convertimos ID a "Mod/Nombre"

            if (loreData != null && loreData.TryGetValue(key, out var entry))
            {
                return entry;
            }

            // Si no hay lore en el JSON, devolvemos uno genérico usando el nombre real del NPC
            return new LoreEntry 
            { 
                title = Lang.GetNPCNameValue(npcId), // Nombre oficial de Terraria
                lore = "Un nuevo habitante ha llegado." 
            };
        }

        // Helper para convertir ID -> "Terraria/Merchant"
        private static string GetNPCKey(int type)
        {
            ModNPC modNpc = ModContent.GetModNPC(type);
            if (modNpc != null)
                return $"{modNpc.Mod.Name}/{modNpc.Name}";

            return $"Terraria/{NPCID.Search.GetName(type)}";
        }
    }

    public class LoreEntry
    {
        public string title { get; set; }
        public string lore { get; set; }
        
        // Propiedad estática por defecto eliminada para evitar confusiones, 
        // usamos la generación dinámica arriba.
    }
}