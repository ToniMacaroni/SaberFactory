using SaberFactory.Models.PropHandler;
using UnityEngine;

namespace SaberFactory.Instances.Setters
{
    internal class TransformDataSetter
    {
        public float Width
        {
            get => _transformPropertyBlock.Width;
            set
            {
                _gameObject.transform.localScale =
                    new Vector3(_baseWidth.x * value, _baseWidth.y * value, _baseWidth.z);
                _transformPropertyBlock.Width = value;
            }
        }

        private readonly GameObject _gameObject;
        private readonly TransformPropertyBlock _transformPropertyBlock;

        private readonly Vector3 _baseWidth;

        public TransformDataSetter(GameObject gameObject, TransformPropertyBlock transformPropertyBlock)
        {
            _gameObject = gameObject;
            _transformPropertyBlock = transformPropertyBlock;
            _baseWidth = gameObject.transform.localScale;

            //Update width
            Width = Width;
        }
    }
}