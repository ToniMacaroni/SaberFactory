using System.Threading;
using HMUI;
using UnityEngine;

namespace FlowUi.Runtime
{
    public class FlowImageBlur : MonoBehaviour
    {
        [SerializeField]
        private ImageView _coverImage;

        [SerializeField]
        private KawaseBlurRendererSO _kawaseBlurRenderer;

        private Texture2D _blurredCoverTexture;

        public ImageView CoverImage
        {
            get => _coverImage;
            set => _coverImage = value;
        }

        protected void OnDestroy()
        {
            if (_blurredCoverTexture != null)
            {
                Destroy(_blurredCoverTexture);
                _blurredCoverTexture = null;
            }
        }

        public void SetImage(Texture2D tex, KawaseBlurRendererSO.KernelSize kernelSize = KawaseBlurRendererSO.KernelSize.Kernel23)
        {
            if (_blurredCoverTexture != null)
            {
                Destroy(_blurredCoverTexture);
            }
            _blurredCoverTexture = _kawaseBlurRenderer.Blur(tex, kernelSize);
            Sprite sprite2 = Sprite.Create(_blurredCoverTexture, new Rect(0f, 0f, _blurredCoverTexture.width, _blurredCoverTexture.height), new Vector2(0.5f, 0.5f), 256f, 0u, SpriteMeshType.FullRect, new Vector4(0f, 0f, 0f, 0f), false);
            if (_coverImage.sprite != null)
            {
                Destroy(_coverImage.sprite);
            }
            _coverImage.sprite = sprite2;
            _coverImage.enabled = true;
        }
    }
}