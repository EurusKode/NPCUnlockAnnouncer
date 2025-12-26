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

            if (Language.Exists(titleKey))
            {
                entry.Title = Language.GetTextValue(titleKey);
                entry.Subtitle = Language.GetTextValue(subtitleKey);
            }
            else
            {
                entry.Title = Lang.GetNPCNameValue(npcId).ToUpper();
                entry.Subtitle = Language.GetTextValue("Mods.NPCUnlockAnnouncer.NPCTitles.Default.Subtitle");
            }

            return entry;
        }
    }
}