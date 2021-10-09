using System;
using BeatSaberMarkupLanguage;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SaberFactory.DataStore
{
    internal class TextureAsset : IDisposable
    {
        public Sprite Sprite => _cachedSprite ??= CreateSprite();
        public bool IsInUse;
        public string Name;
        public EAssetOrigin Origin;
        public string Path;
        public Texture2D Texture;

        private Sprite _cachedSprite;

        public TextureAsset(string name, string path, Texture2D texture, EAssetOrigin origin)
        {
            Name = name;
            Path = path;
            Texture = texture;
            
            if (name.ToLower().Contains("_clamp"))
            {
                Texture.wrapMode = TextureWrapMode.Clamp;
            }
            
            Origin = origin;
        }

        public void Dispose()
        {
            Object.Destroy(Texture);
            Object.Destroy(_cachedSprite);
        }

        private Sprite CreateSprite()
        {
            return Utilities.LoadSpriteFromTexture(Texture);
        }

        public void Dispose(bool forced)
        {
            if (forced)
            {
                Dispose();
            }
            else
            {
                if (!IsInUse) Dispose();
            }
        }
    }
}