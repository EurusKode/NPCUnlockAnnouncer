using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace NPCUnlockAnnouncer.UI
{
    public class NPCUnlockUISystem : ModSystem
    {
        public static NPCUnlockUISystem Instance;
        private NPCUnlockAnimator animator;
        private int currentNpcType;
        private string currentTitle;
        private string currentSubtitle;
        private Queue<(int type, string title, string sub)> _unlockQueue = new Queue<(int, string, string)>();

        public override void Load()
        {
            if (Main.dedServ) return;
            Instance = this;
            animator = new NPCUnlockAnimator();
        }

        public void ShowNPCUnlock(int npcType, string title, string subtitle)
        {
            _unlockQueue.Enqueue((npcType, title, subtitle));
        }

        public override void PostDrawInterface(SpriteBatch spriteBatch)
        {
            animator.Update();

            if (!animator.IsActive)
            {
                if (_unlockQueue.Count > 0)
                {
                    var next = _unlockQueue.Dequeue();
                    
                    this.currentNpcType = next.type;
                    this.currentTitle = next.title;
                    this.currentSubtitle = next.sub;

                    animator.Start();
                }
                else
                {
                    return;
                }
            }

            float y = animator.CurrentY;
            float width = 420f;
            float height = 64f;
            float x = (Main.screenWidth - width) / 2f;

            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)x, (int)y, (int)width, (int)height), new Color(20, 20, 20, 230));
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)x, (int)y, (int)width, 2), Color.Gold);
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)x, (int)y + (int)height - 2, (int)width, 2), Color.Gold);

            int headIndex = NPC.TypeToDefaultHeadIndex(currentNpcType);
            if (headIndex != -1)
            {
                Texture2D headTexture = TextureAssets.NpcHead[headIndex].Value;
                Vector2 iconCenter = new Vector2(x + 32, y + 32);
                Vector2 origin = headTexture.Size() / 2f;
                spriteBatch.Draw(headTexture, iconCenter, null, Color.White, 0f, origin, 1.2f, SpriteEffects.None, 0f);
            }

            Utils.DrawBorderString(spriteBatch, currentTitle.ToUpper(), new Vector2(x + 70, y + 8), Color.Gold, 1.1f);
            Utils.DrawBorderString(spriteBatch, currentSubtitle, new Vector2(x + 70, y + 34), Color.LightGray, 0.85f);
        }
    }
}