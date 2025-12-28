using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace NPCUnlockAnnouncer.Data
{
    public static class LoreDatabase
    {
        public struct LoreEntry
        {
            public string Title;
            public string Subtitle;
        }

        public static LoreEntry GetLore(int npcId)
        {
            LoreEntry entry = new LoreEntry();
            string internalName = NPCID.Search.GetName(npcId);
            string baseKey = $"Mods.NPCUnlockAnnouncer.NPCTitles.{internalName}";
            string titleKey = baseKey + ".Title";
            string subtitleKey = baseKey + ".Subtitle";

            // 1. Obtener Título
            if (Language.Exists(titleKey))
            {
                entry.Title = Language.GetTextValue(titleKey);
            }
            else
            {
                entry.Title = Lang.GetNPCNameValue(npcId).ToUpper();
            }

            // 2. Obtener Subtítulo y aplicar la lógica de ROTACIÓN (Aleatorio)
            string rawSubtitle = "";
            if (Language.Exists(subtitleKey))
            {
                rawSubtitle = Language.GetTextValue(subtitleKey);
            }
            else
            {
                rawSubtitle = Language.GetTextValue("Mods.NPCUnlockAnnouncer.NPCTitles.Default.Subtitle");
            }

            // Aquí ocurre la magia: Cortamos por la barra '|' y elegimos uno al azar
            if (!string.IsNullOrEmpty(rawSubtitle))
            {
                string[] variants = rawSubtitle.Split('|');
                entry.Subtitle = variants[Main.rand.Next(variants.Length)];
            }
            else
            {
                entry.Subtitle = "New NPC Unlocked!";
            }

            return entry;
        }
    }
}