using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using NPCUnlockAnnouncer.UI; 
using NPCUnlockAnnouncer.Data;

namespace NPCUnlockAnnouncer.Systems
{
    public class UnlockDetectionSystem : ModSystem
    {
        private HashSet<int> _unlockedNPCs = new HashSet<int>();
        private bool _isInitialized = false;

        public override void OnWorldLoad()
        {
            _unlockedNPCs.Clear();
            _isInitialized = false;
        }

        public override void PostUpdateNPCs()
        {
            if (!_isInitialized)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].active && Main.npc[i].townNPC)
                    {
                        _unlockedNPCs.Add(Main.npc[i].type);
                    }
                }
                _isInitialized = true;
                return;
            }

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                if (npc.active && npc.townNPC && !_unlockedNPCs.Contains(npc.type))
                {
                    _unlockedNPCs.Add(npc.type);
                    TriggerUnlockAnimation(npc);
                }
            }
        }

        private void TriggerUnlockAnimation(NPC npc)
        {
            var datos = LoreDatabase.GetLore(npc.type);

            if (NPCUnlockUISystem.Instance != null)
            {
                NPCUnlockUISystem.Instance.ShowNPCUnlock(npc.type, datos.Title, datos.Subtitle);
            }
        }
    }
}