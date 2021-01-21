using BeatSaberMarkupLanguage;
using UnityEngine;

namespace SaberFactory
{
    internal class CommonResources
    {
        public readonly Sprite DefaultCover;

        private CommonResources()
        {
            DefaultCover = Utilities.LoadSpriteFromTexture(new Texture2D(2, 2));
        }
    }
}