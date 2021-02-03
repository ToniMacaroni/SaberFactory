using System;
using BeatSaberMarkupLanguage;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SaberFactory.DataStore
{
    internal class TextureAsset : IDisposable
    {
        public string Name;
        public string Path;
        public Texture2D Texture;
        public EAssetOrigin Origin;
        public Sprite Sprite => _cachedSprite ??= CreateSprite();
        public bool IsInUse;

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

        public void Dispose()
        {
            Object.Destroy(Texture);
            Object.Destroy(_cachedSprite);
        }

        public void Dispose(bool forced)
        {
            if(forced) Dispose();
            else
            {
                if(!IsInUse) Dispose();
            }
        }
    }
}