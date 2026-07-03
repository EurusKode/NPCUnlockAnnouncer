using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID; // Necesario para NPCID
using Terraria.ModLoader;
using Terraria.Audio;
using NPCUnlockAnnouncer.Systems;

namespace NPCUnlockAnnouncer.UI
{
    public class NPCUnlockUISystem : ModSystem
    {
        public static NPCUnlockUISystem Instance;
        private NPCUnlockAnimator animator;
        
        // Datos del NPC actual
        private int currentNpcType;
        private string currentTitle;
        private string currentSubtitle;
        
        // Cola de espera
        private Queue<(int type, string title, string sub)> _unlockQueue = new Queue<(int, string, string)>();

        // VARIABLES PARA ANIMACIÓN
        private int _frameCounter = 0;
        private int _currentFrame = 0;

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

        private void PlayUnlockSound()
        {
            var config = AnnouncerConfig.Instance;
            if (config == null || config.Sound == SoundPreset.None) return;

            SoundStyle? style = null;
            switch (config.Sound)
            {
                case SoundPreset.MagicDing:
                    style = SoundID.Item29;
                    break;
                case SoundPreset.Coins:
                    style = SoundID.Coins;
                    break;
                case SoundPreset.MaxMana:
                    style = SoundID.MaxMana;
                    break;
                case SoundPreset.ResearchComplete:
                    style = SoundID.ResearchComplete;
                    break;
                case SoundPreset.ChestUnlock:
                    style = SoundID.Unlock;
                    break;
                case SoundPreset.Crimson:
                    style = new SoundStyle("NPCUnlockAnnouncer/Assets/Sounds/Crimson");
                    break;
                case SoundPreset.Knock:
                    style = new SoundStyle("NPCUnlockAnnouncer/Assets/Sounds/Knock");
                    break;
                case SoundPreset.Jungle:
                    style = new SoundStyle("NPCUnlockAnnouncer/Assets/Sounds/Jungle");
                    break;
                case SoundPreset.Corruption:
                    style = new SoundStyle("NPCUnlockAnnouncer/Assets/Sounds/Corruption");
                    break;
            }

            if (style.HasValue)
            {
                SoundEngine.PlaySound(style.Value with { Volume = config.SoundVolume / 100f });
            }
        }

        public override void PostDrawInterface(SpriteBatch spriteBatch)
        {
            animator.Update();

            if (!animator.IsActive)
            {
                _frameCounter = 0;
                _currentFrame = 0;
                if (_unlockQueue.Count > 0)
                {
                    var next = _unlockQueue.Dequeue();
                    this.currentNpcType = next.type;
                    this.currentTitle = next.title;
                    this.currentSubtitle = next.sub;
                    animator.Start();
                    PlayUnlockSound();
                }
                else return;
            }

            float y = animator.CurrentY;

            var config = AnnouncerConfig.Instance;
            NotificationStyle activeStyle = config != null ? config.Style : NotificationStyle.Classic;

            // --- 1. INTENTAR CARGAR IMÁGENES ---
            Texture2D backgroundTexture = null;
            int frameCount = 1;
            bool usingCustomImage = false;

            if (activeStyle == NotificationStyle.Crimson)
            {
                string crimsonPath = "NPCUnlockAnnouncer/Assets/Crimson";
                if (ModContent.HasAsset(crimsonPath))
                {
                    backgroundTexture = ModContent.Request<Texture2D>(crimsonPath).Value;
                }
            }
            else if (activeStyle == NotificationStyle.Wood)
            {
                string woodPath = "NPCUnlockAnnouncer/Assets/Wood";
                if (ModContent.HasAsset(woodPath))
                {
                    backgroundTexture = ModContent.Request<Texture2D>(woodPath).Value;
                }
            }
            else if (activeStyle == NotificationStyle.Jungle)
            {
                string junglePath = "NPCUnlockAnnouncer/Assets/Jungle";
                if (ModContent.HasAsset(junglePath))
                {
                    backgroundTexture = ModContent.Request<Texture2D>(junglePath).Value;
                }
            }
            else if (activeStyle == NotificationStyle.Corruption)
            {
                string corruptionPath = "NPCUnlockAnnouncer/Assets/Corruption";
                if (ModContent.HasAsset(corruptionPath))
                {
                    backgroundTexture = ModContent.Request<Texture2D>(corruptionPath).Value;
                }
            }

            // --- 2. DIBUJADO (Con Red de Seguridad) ---
            float x;
            
            if (activeStyle == NotificationStyle.Modern)
            {
                // === OPCIÓN C: ESTILO MODERNO (Neon Glassmorphic) ===
                float width = 420f;
                float height = 64f;
                x = (Main.screenWidth - width) / 2f;

                // Dynamic neon colors cycling (Cyan to electric Purple/Magenta)
                float colorCycle = (float)System.Math.Sin(Main.GlobalTimeWrappedHourly * 4f) * 0.5f + 0.5f;
                Color neonColor = Color.Lerp(new Color(0, 230, 255), new Color(220, 0, 255), colorCycle);

                // Background Outer soft glow
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)x - 4, (int)y - 4, (int)width + 8, (int)height + 8), neonColor * 0.12f);
                // Background Inner soft glow
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)x - 2, (int)y - 2, (int)width + 4, (int)height + 4), neonColor * 0.25f);
                // Main dark panel background
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)x, (int)y, (int)width, (int)height), new Color(12, 16, 26, 220));

                // Draw neon borders
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)x, (int)y, (int)width, 2), neonColor);
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)x, (int)y + (int)height - 2, (int)width, 2), neonColor);
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)x, (int)y, 2, (int)height), neonColor);
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)x + (int)width - 2, (int)y, 2, (int)height), neonColor);

                // NPC head frame background and borders
                int headIndex = NPC.TypeToDefaultHeadIndex(currentNpcType);
                if (headIndex != -1)
                {
                    Texture2D headTexture = TextureAssets.NpcHead[headIndex].Value;
                    Vector2 iconCenter = new Vector2(x + 36, y + 32);
                    
                    Rectangle iconFrame = new Rectangle((int)x + 14, (int)y + 12, 44, 40);
                    spriteBatch.Draw(TextureAssets.MagicPixel.Value, iconFrame, new Color(22, 30, 44, 255));
                    
                    spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(iconFrame.X, iconFrame.Y, iconFrame.Width, 1), neonColor * 0.6f);
                    spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(iconFrame.X, iconFrame.Y + iconFrame.Height - 1, iconFrame.Width, 1), neonColor * 0.6f);
                    spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(iconFrame.X, iconFrame.Y, 1, iconFrame.Height), neonColor * 0.6f);
                    spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(iconFrame.X + iconFrame.Width - 1, iconFrame.Y, 1, iconFrame.Height), neonColor * 0.6f);

                    Vector2 origin = headTexture.Size() / 2f;
                    spriteBatch.Draw(headTexture, iconCenter, null, Color.White, 0f, origin, 1.1f, SpriteEffects.None, 0f);
                }
            }
            else if (activeStyle == NotificationStyle.Crimson && backgroundTexture != null)
            {
                // === ESTILO CRIMSON ===
                float width = backgroundTexture.Width;
                float height = backgroundTexture.Height;
                x = (Main.screenWidth - width) / 2f;

                // Dibujar Banner
                spriteBatch.Draw(backgroundTexture, new Vector2(x, y), Color.White);

                // Dibujar Cabeza (Centrada en el círculo izquierdo)
                int headIndex = NPC.TypeToDefaultHeadIndex(currentNpcType);
                if (headIndex != -1)
                {
                    Texture2D headTexture = TextureAssets.NpcHead[headIndex].Value;
                    Vector2 iconCenter = new Vector2(x + 36, y + (height / 2f));
                    Vector2 origin = headTexture.Size() / 2f;
                    spriteBatch.Draw(headTexture, iconCenter, null, Color.White, 0f, origin, 1.15f, SpriteEffects.None, 0f);
                }
            }
            else if (activeStyle == NotificationStyle.Wood && backgroundTexture != null)
            {
                // === ESTILO WOOD ===
                float width = backgroundTexture.Width;
                float height = backgroundTexture.Height;
                x = (Main.screenWidth - width) / 2f;

                // Dibujar Banner
                spriteBatch.Draw(backgroundTexture, new Vector2(x, y), Color.White);

                // Dibujar Cabeza (Centrada a la izquierda)
                int headIndex = NPC.TypeToDefaultHeadIndex(currentNpcType);
                if (headIndex != -1)
                {
                    Texture2D headTexture = TextureAssets.NpcHead[headIndex].Value;
                    Vector2 iconCenter = new Vector2(x + 36, y + (height / 2f));
                    Vector2 origin = headTexture.Size() / 2f;
                    spriteBatch.Draw(headTexture, iconCenter, null, Color.White, 0f, origin, 1.15f, SpriteEffects.None, 0f);
                }
            }
            else if (activeStyle == NotificationStyle.Jungle && backgroundTexture != null)
            {
                // === ESTILO JUNGLE ===
                float width = backgroundTexture.Width;
                float height = backgroundTexture.Height;
                x = (Main.screenWidth - width) / 2f;

                // Dibujar Banner
                spriteBatch.Draw(backgroundTexture, new Vector2(x, y), Color.White);

                // Dibujar Cabeza (Centrada a la izquierda)
                int headIndex = NPC.TypeToDefaultHeadIndex(currentNpcType);
                if (headIndex != -1)
                {
                    Texture2D headTexture = TextureAssets.NpcHead[headIndex].Value;
                    Vector2 iconCenter = new Vector2(x + 36, y + (height / 2f));
                    Vector2 origin = headTexture.Size() / 2f;
                    spriteBatch.Draw(headTexture, iconCenter, null, Color.White, 0f, origin, 1.15f, SpriteEffects.None, 0f);
                }
            }
            else if (activeStyle == NotificationStyle.Corruption && backgroundTexture != null)
            {
                // === ESTILO CORRUPTION ===
                float width = backgroundTexture.Width;
                float height = backgroundTexture.Height;
                x = (Main.screenWidth - width) / 2f;

                // Dibujar Banner
                spriteBatch.Draw(backgroundTexture, new Vector2(x, y), Color.White);

                // Dibujar Cabeza (Centrada a la izquierda)
                int headIndex = NPC.TypeToDefaultHeadIndex(currentNpcType);
                if (headIndex != -1)
                {
                    Texture2D headTexture = TextureAssets.NpcHead[headIndex].Value;
                    Vector2 iconCenter = new Vector2(x + 36, y + (height / 2f));
                    Vector2 origin = headTexture.Size() / 2f;
                    spriteBatch.Draw(headTexture, iconCenter, null, Color.White, 0f, origin, 1.15f, SpriteEffects.None, 0f);
                }
            }
            else if (usingCustomImage)
            {
                // === OPCIÓN A: DIBUJAR IMAGEN PNG ===
                
                // Lógica de animación
                if (frameCount > 1)
                {
                    _frameCounter++;
                    if (_frameCounter >= 6)
                    {
                        _frameCounter = 0;
                        _currentFrame++;
                        if (_currentFrame >= frameCount) _currentFrame = 0;
                    }
                }
                else _currentFrame = 0;

                int frameHeight = backgroundTexture.Height / frameCount;
                Rectangle sourceRect = new Rectangle(0, _currentFrame * frameHeight, backgroundTexture.Width, frameHeight);

                x = (Main.screenWidth - backgroundTexture.Width) / 2f;
                Vector2 bannerPos = new Vector2(x, y);

                // Dibujar Banner
                spriteBatch.Draw(backgroundTexture, bannerPos, sourceRect, Color.White);

                // Dibujar Cabeza (Solo si usas imagen custom, la colocamos bonita)
                int headIndex = NPC.TypeToDefaultHeadIndex(currentNpcType);
                if (headIndex != -1)
                {
                    Texture2D headTexture = TextureAssets.NpcHead[headIndex].Value;
                    // Ajusta este (+50, +frameHeight/2) para mover la cabeza a donde quieras en TU imagen
                    Vector2 iconCenter = new Vector2(x + 50, y + (frameHeight / 2f)); 
                    Vector2 origin = headTexture.Size() / 2f;
                    spriteBatch.Draw(headTexture, iconCenter, null, Color.White, 0f, origin, 1.2f, SpriteEffects.None, 0f);
                }
            }
            else
            {
                // === OPCIÓN B: FALLBACK (El cuadro negro clásico) ===
                // Esto se activa si NO tienes las imágenes en la carpeta Assets todavía.
                
                float width = 420f;
                float height = 64f;
                x = (Main.screenWidth - width) / 2f;

                // Fondo negro
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)x, (int)y, (int)width, (int)height), new Color(20, 20, 20, 230));
                // Bordes dorados
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)x, (int)y, (int)width, 2), Color.Gold);
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)x, (int)y + (int)height - 2, (int)width, 2), Color.Gold);

                // Cabeza Estándar
                int headIndex = NPC.TypeToDefaultHeadIndex(currentNpcType);
                if (headIndex != -1)
                {
                    Texture2D headTexture = TextureAssets.NpcHead[headIndex].Value;
                    Vector2 iconCenter = new Vector2(x + 32, y + 32);
                    Vector2 origin = headTexture.Size() / 2f;
                    spriteBatch.Draw(headTexture, iconCenter, null, Color.White, 0f, origin, 1.2f, SpriteEffects.None, 0f);
                }
            }

            // --- 3. DIBUJAR TEXTOS (Común para ambos casos) ---
            // Usamos offsets relativos a 'x' para que sirva en todos los modos
            
            float textOffsetX;
            float textOffsetY;
            float subOffsetY;
            Color titleColor;
            Color subtitleColor;

            if (activeStyle == NotificationStyle.Modern)
            {
                textOffsetX = 72f;
                textOffsetY = 10f;
                subOffsetY = 36f;
                titleColor = new Color(0, 230, 255);
                subtitleColor = new Color(200, 220, 240);
            }
            else if (activeStyle == NotificationStyle.Crimson)
            {
                textOffsetX = 76f;
                textOffsetY = 14f;
                subOffsetY = 42f;
                titleColor = new Color(255, 60, 60); // Bloody / Crimson Red
                subtitleColor = new Color(240, 180, 180); // Flesh Pink / Light Red
            }
            else if (activeStyle == NotificationStyle.Wood)
            {
                textOffsetX = 76f;
                textOffsetY = 14f;
                subOffsetY = 42f;
                titleColor = new Color(245, 190, 100); // Warm Golden Wood
                subtitleColor = new Color(190, 230, 150); // Moss Green / Light Green
            }
            else if (activeStyle == NotificationStyle.Jungle)
            {
                textOffsetX = 76f;
                textOffsetY = 14f;
                subOffsetY = 42f;
                titleColor = new Color(255, 170, 50); // Orange / Flower Orange
                subtitleColor = new Color(160, 235, 120); // Bright Jungle Green
            }
            else if (activeStyle == NotificationStyle.Corruption)
            {
                textOffsetX = 76f;
                textOffsetY = 14f;
                subOffsetY = 42f;
                titleColor = new Color(190, 110, 255); // Corruption Purple
                subtitleColor = new Color(220, 195, 245); // Lilac / Light Purple-Grey
            }
            else if (usingCustomImage)
            {
                textOffsetX = 100f;
                textOffsetY = 15f;
                subOffsetY = 40f;
                titleColor = Color.Gold;
                subtitleColor = Color.LightGray;
            }
            else
            {
                textOffsetX = 70f;
                textOffsetY = 8f;
                subOffsetY = 34f;
                titleColor = Color.Gold;
                subtitleColor = Color.LightGray;
            }

            Utils.DrawBorderString(spriteBatch, currentTitle.ToUpper(), new Vector2(x + textOffsetX, y + textOffsetY), titleColor, 1.1f);
            Utils.DrawBorderString(spriteBatch, currentSubtitle, new Vector2(x + textOffsetX, y + subOffsetY), subtitleColor, 0.85f);
        }
    }
}