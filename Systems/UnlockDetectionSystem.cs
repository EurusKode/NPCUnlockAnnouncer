using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using NPCUnlockAnnouncer.UI; 
using NPCUnlockAnnouncer.Data; // Importante para LoreDatabase

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
            // Inicialización: Llenar lista con NPCs que ya estaban al entrar al mundo
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

            // Detección en tiempo real
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
            // 1. Obtenemos los datos del Lore usando el ID
            LoreEntry datos = LoreDatabase.GetLore(npc.type);

            // 2. Llamamos al sistema de UI
            // Nota: Usamos 'Instance' si lo definiste como static en NPCUnlockUISystem,
            // o ModContent.GetInstance si no. Tu código actual usa Instance.
            if (NPCUnlockUISystem.Instance != null)
            {
                // Pasamos ID, Título y Lore (Subtítulo)
                NPCUnlockUISystem.Instance.ShowNPCUnlock(npc.type, datos.title, datos.lore);
            }
        }
    }
}