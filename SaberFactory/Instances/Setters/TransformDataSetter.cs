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

        public float Offset
        {
            get => _transformPropertyBlock.Offset;
            set
            {
                var pos = _transform.localPosition;
                pos.z = value;
                _transform.localPosition = pos;
                _transformPropertyBlock.Offset = value;
            }
        }

        public float Rotation
        {
            get => _transformPropertyBlock.Rotation;
            set
            {
                var rot = _transform.localEulerAngles;
                rot.z = value;
                _transform.localEulerAngles = rot;
                _transformPropertyBlock.Rotation = value;
            }
        }

        private readonly GameObject _gameObject;
        private readonly Transform _transform;
        private readonly TransformPropertyBlock _transformPropertyBlock;

        private readonly Vector3 _baseWidth;

        public TransformDataSetter(GameObject gameObject, TransformPropertyBlock transformPropertyBlock)
        {
            _gameObject = gameObject;
            _transform = gameObject.transform;
            _transformPropertyBlock = transformPropertyBlock;
            _baseWidth = gameObject.transform.localScale;

            //Update props
            Width = Width;
            Offset = Offset;
            Rotation = Rotation;
        }
    }
}