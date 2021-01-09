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
                if(_instance) _instance.SetActive(value);
            }
        }

        private readonly EmbeddedAssetLoader _embeddedAssetLoader;

        private GameObject _instance;

        public Pedestal(EmbeddedAssetLoader embeddedAssetLoader)
        {
            _embeddedAssetLoader = embeddedAssetLoader;
        }

        public async Task Instantiate(Vector3 pos, Quaternion rot)
        {
            if (_instance) return;
            var prefab = await _embeddedAssetLoader.LoadAsset<GameObject>("Pedestal");
            if (!prefab) return;
            _instance = Object.Instantiate(prefab, pos, rot);
            IsVisible = false;
        }

        public void Destroy()
        {
            _instance.TryDestroy();
        }
    }
}