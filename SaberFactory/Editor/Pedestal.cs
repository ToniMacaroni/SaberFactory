using System;
using System.IO;
using System.Threading.Tasks;
using SaberFactory.Helpers;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SaberFactory.Editor
{
    internal class Pedestal
    {
        public bool IsVisible
        {
            set
            {
                if (_rootTransform)
                {
                    _rootTransform.gameObject.SetActive(value);
                }
            }
        }

        public Vector3 Position
        {
            get => _rootTransform.position;
            set => _rootTransform.position = value;
        }

        public Transform SaberContainerTransform { get; private set; }
        private readonly FileInfo _customPedestalFile;

        private readonly EmbeddedAssetLoader _embeddedAssetLoader;

        private Transform _rootTransform;

        public Pedestal(EmbeddedAssetLoader embeddedAssetLoader, FileInfo customPedestalFile)
        {
            _embeddedAssetLoader = embeddedAssetLoader;
            _customPedestalFile = customPedestalFile;
        }

        public async Task Instantiate(Vector3 pos, Quaternion rot)
        {
            if (_rootTransform)
            {
                return;
            }

            _rootTransform = new GameObject("Pedestal Container").transform;

            var prefab = await GetPedestalAsset();
            if (!prefab)
            {
                return;
            }

            Object.Instantiate(prefab, _rootTransform, false);

            SaberContainerTransform = _rootTransform.CreateGameObject("SaberContainer").transform;
            SaberContainerTransform.localPosition += new Vector3(0, 1, 0);
            SaberContainerTransform.localEulerAngles = new Vector3(-90, 0, 0);

            _rootTransform.SetPositionAndRotation(pos, rot);


            IsVisible = false;
        }

        private async Task<GameObject> GetPedestalAsset()
        {
            if (_customPedestalFile.Exists)
            {
                try
                {
                    var customPedestal = await Readers.LoadAssetFromAssetBundleAsync<GameObject>(_customPedestalFile.FullName, "Pedestal");
                    customPedestal.Item2.Unload(false);
                    return customPedestal.Item1;
                }
                catch (Exception e)
                {
                    Debug.LogError("Couldn't load custom pedestal: \n" + e);
                }
            }

            return await _embeddedAssetLoader.LoadAsset<GameObject>("Pedestal");
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