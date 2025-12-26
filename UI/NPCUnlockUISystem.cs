using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace NPCUnlockAnnouncer.UI
{
    public class NPCUnlockUISystem : ModSystem
    {
        public static NPCUnlockUISystem Instance;
        private NPCUnlockAnimator animator;

        private int npcType;
        private string title;
        private string subtitle;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            Instance = this;
            animator = new NPCUnlockAnimator();
        }

        public void ShowNPCUnlock(int npcType, string title, string subtitle)
        {
            this.npcType = npcType;
            this.title = title;
            this.subtitle = subtitle;
            animator.Start();
        }

        public override void PostDrawInterface(SpriteBatch spriteBatch)
        {
            animator.Update();

            if (!animator.IsActive)
                return;

            float y = animator.CurrentY;
            float width = 420f;
            float height = 64f;
            float x = (Main.screenWidth - width) / 2f;

            // 1. DIBUJAR FONDO Y BORDE
            spriteBatch.Draw(
                TextureAssets.MagicPixel.Value,
                new Rectangle((int)x, (int)y, (int)width, (int)height),
                new Color(20, 20, 20, 230)
            );

            // Bordes dorados
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)x, (int)y, (int)width, 2), Color.Gold);
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)x, (int)y + (int)height - 2, (int)width, 2), Color.Gold);

            // 2. DIBUJAR ÍCONO DE CABEZA (CORREGIDO)
            Vector2 iconCenter = new Vector2(x + 32, y + 32); 

            // Obtenemos el ID de la cabeza
            int headIndex = NPC.TypeToDefaultHeadIndex(npcType);

            if (headIndex != -1)
            {
                // --- CAMBIO AQUÍ: Eliminamos la línea Main.instance.LoadNPCHead ---
                // Simplemente accedemos al valor, tModLoader lo carga solo.
                Texture2D headTexture = TextureAssets.NpcHead[headIndex].Value;

                Vector2 origin = headTexture.Size() / 2f;
                float scale = 1.2f; 

                spriteBatch.Draw(
                    headTexture,
                    iconCenter,
                    null,
                    Color.White,
                    0f,
                    origin,
                    scale,
                    SpriteEffects.None,
                    0f
                );
            }

            // 3. DIBUJAR TEXTOS
            Utils.DrawBorderString(
                spriteBatch,
                title.ToUpper(),
                new Vector2(x + 70, y + 8), 
                Color.Gold,
                1.1f
            );

            Utils.DrawBorderString(
                spriteBatch,
                subtitle,
                new Vector2(x + 70, y + 34),
                Color.LightGray,
                0.85f
            );
        }
    }
}