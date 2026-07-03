using System.Collections.Generic;
using System.ComponentModel;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace NPCUnlockAnnouncer.Systems
{
    public enum NotificationStyle
    {
        Classic,
        Modern,
        Corruption,
        Crimson,
        Wood,
        Jungle
    }

    public enum SoundPreset
    {
        None,
        MagicDing,
        Coins,
        MaxMana,
        ResearchComplete,
        ChestUnlock,
        Crimson,
        Knock,
        Jungle,
        Corruption
    }

    public class NPCMessageConfig
    {
        public NPCDefinition NPC = new NPCDefinition();
        public string Title = "";
        public string Subtitle = "";

        public override string ToString()
        {
            return NPC != null && NPC.Type > 0 ? NPC.ToString() : "NPC";
        }
    }

    public class AnnouncerConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        public static AnnouncerConfig Instance => ModContent.GetInstance<AnnouncerConfig>();

        [Header("GeneralSettings")]
        [DrawTicks]
        [DefaultValue(NotificationStyle.Classic)]
        public NotificationStyle Style;

        [DrawTicks]
        [DefaultValue(SoundPreset.MagicDing)]
        public SoundPreset Sound;

        [Range(0, 100)]
        [Increment(5)]
        [DefaultValue(100)]
        public int SoundVolume;

        [Range(1, 15)]
        [Increment(1)]
        [DefaultValue(5)]
        public int DurationSeconds;

        [Header("CustomMessages")]
        public List<NPCMessageConfig> CustomNPCMessages = new();
    }
}
