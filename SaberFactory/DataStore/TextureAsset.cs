using BeatSaberMarkupLanguage;
using UnityEngine;

namespace SaberFactory.DataStore
{
    internal class TextureAsset
    {
        public string Name;
        public string Path;
        public Texture2D Texture;
        public EAssetOrigin Origin;
        public Sprite Sprite => _cachedSprite ??= CreateSprite();

        private Sprite _cachedSprite;

        public TextureAsset(string name, string path, Texture2D texture, EAssetOrigin origin)
        {
            Name = name;
            Path = path;
            Texture = texture;
            Origin = origin;
        }

        private Sprite CreateSprite()
        {
            return Utilities.LoadSpriteFromTexture(Texture);
        }
    }
}