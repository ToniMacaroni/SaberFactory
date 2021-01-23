using System.Threading.Tasks;
using SaberFactory.Helpers;
using UnityEngine;

namespace SaberFactory.Editor
{
    internal class Pedestal
    {
        public bool IsVisible
        {
            set
            {
                if(_rootTransform) _rootTransform.gameObject.SetActive(value);
            }
        }

        public Vector3 Position
        {
            get => _rootTransform.position;
            set => _rootTransform.position = value;
        }

        public Transform SaberContainerTransform { get; private set; }

        private readonly EmbeddedAssetLoader _embeddedAssetLoader;

        private Transform _rootTransform;

        public Pedestal(EmbeddedAssetLoader embeddedAssetLoader)
        {
            _embeddedAssetLoader = embeddedAssetLoader;
        }

        public async Task Instantiate(Vector3 pos, Quaternion rot)
        {
            if (_rootTransform) return;
            _rootTransform = new GameObject("Pedestal Container").transform;

            var prefab = await _embeddedAssetLoader.LoadAsset<GameObject>("Pedestal");
            if (!prefab) return;
            Object.Instantiate(prefab, _rootTransform, false);

            SaberContainerTransform = _rootTransform.CreateGameObject("SaberContainer").transform;
            SaberContainerTransform.localPosition += new Vector3(0, 1, 0);
            SaberContainerTransform.localEulerAngles = new Vector3(-90, 0, 0);

            _rootTransform.position = pos;
            _rootTransform.rotation = rot;

            IsVisible = false;
        }

        public void Destroy()
        {
            if (_rootTransform != null)
            {
                _rootTransform.gameObject.TryDestroy();
            }
        }
    }
}