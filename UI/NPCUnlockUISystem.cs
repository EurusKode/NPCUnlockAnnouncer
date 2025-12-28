using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID; // Necesario para NPCID
using Terraria.ModLoader;

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
        }
        else return;
    }

    float y = animator.CurrentY;

    // --- 1. INTENTAR CARGAR IMÁGENES ---
    string npcName = NPCID.Search.GetName(currentNpcType);
    string specificPath = "NPCUnlockAnnouncer/Assets/" + npcName;
    string genericPath = "NPCUnlockAnnouncer/Assets/Generic";

    Texture2D backgroundTexture = null;
    int frameCount = 1;
    bool usingCustomImage = false;

    // A) ¿Existe imagen específica (Ej: Merchant.png)?
    if (ModContent.HasAsset(specificPath))
    {
        backgroundTexture = ModContent.Request<Texture2D>(specificPath).Value;
        usingCustomImage = true;
        // Configura aquí si alguno tiene animación especial
        if (currentNpcType == NPCID.Merchant) frameCount = 4; 
    }
    // B) ¿Existe imagen genérica (Generic.png)?
    else if (ModContent.HasAsset(genericPath))
    {
        backgroundTexture = ModContent.Request<Texture2D>(genericPath).Value;
        usingCustomImage = true;
        frameCount = 1; // Cambia esto si tu genérico es animado
    }

    // --- 2. DIBUJADO (Con Red de Seguridad) ---
    float x;
    
    if (usingCustomImage)
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
    // Usamos offsets relativos a 'x' para que sirva en ambos modos
    
    // Título
    // Si usas imagen, quizás quieras moverlo más a la derecha (ej: x + 100)
    // Si usas cuadro negro, (ej: x + 70)
    float textOffsetX = usingCustomImage ? 100f : 70f;
    float textOffsetY = usingCustomImage ? 15f : 8f;

    Utils.DrawBorderString(spriteBatch, currentTitle.ToUpper(), new Vector2(x + textOffsetX, y + textOffsetY), Color.Gold, 1.1f);

    // Subtítulo
    float subOffsetY = usingCustomImage ? 40f : 34f;
    Utils.DrawBorderString(spriteBatch, currentSubtitle, new Vector2(x + textOffsetX, y + subOffsetY), Color.LightGray, 0.85f);
}
    }
}