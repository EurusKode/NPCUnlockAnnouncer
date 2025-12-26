using Terraria.ModLoader;
using NPCUnlockAnnouncer.Data;

namespace NPCUnlockAnnouncer
{
    public class NPCUnlockAnnouncer : Mod
    {
        public override void Load()
        {
            // Load lore database when the mod loads
            LoreDatabase.Load(this);
        }
    }
}
