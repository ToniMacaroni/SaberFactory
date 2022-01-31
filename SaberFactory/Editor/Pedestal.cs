using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SaberFactory.Helpers;
using SaberFactory.Loaders;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SaberFactory.Editor
{
    internal class Pedestal
    {
        private static readonly string PedestalPath = String.Join(".", nameof(SaberFactory), "Resources", "pedestal");

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

        private Transform _rootTransform;
        private TextMeshPro _textMeshPro;
        private Material _ledMat;
        private Material _spiralMat;

        private readonly string[] _lines = new string[3];
        private static readonly int LedColor = Shader.PropertyToID("_LedColor");
        private static readonly int Length = Shader.PropertyToID("_Length");

        public Pedestal(FileInfo customPedestalFile)
        {
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

            var instantiated = Object.Instantiate(prefab, _rootTransform, false);
            _textMeshPro = instantiated.GetComponentsInChildren<TextMeshPro>()
                .FirstOrDefault(x => x.name == "Pedestal_Display");
            var leds = instantiated.GetComponentsInChildren<MeshRenderer>().FirstOrDefault(x => x.name == "Leds");
            var spiral = instantiated.GetComponentsInChildren<MeshRenderer>().FirstOrDefault(x => x.name == "Spiral");

            if (_textMeshPro)
            {
                _textMeshPro.alignment = TextAlignmentOptions.Center;
            }

            if (leds)
            {
                _ledMat = leds.sharedMaterial;
            }

            if (spiral)
            {
                _spiralMat = spiral.sharedMaterial;
            }

            SaberContainerTransform = _rootTransform.CreateGameObject("SaberContainer").transform;
            SaberContainerTransform.localPosition += new Vector3(0, 1, 0);
            SaberContainerTransform.localEulerAngles = new Vector3(-90, 0, 0);

            _rootTransform.SetPositionAndRotation(pos, rot);


            IsVisible = false;
        }

        public void SetText(int line, string text)
        {
            if (!_textMeshPro)
            {
                return;
            }

            _lines[line] = text;
            _textMeshPro.text = string.Join("\n", _lines);
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

            var data = await Readers.ReadResourceAsync(PedestalPath);
            var bundle = await Readers.LoadAssetFromAssetBundleAsync<GameObject>(data, "Pedestal");
            bundle.Item2.Unload(false);
            return bundle.Item1;
        }

        public void Destroy()
        {
            if (_rootTransform != null)
            {
                _rootTransform.gameObject.TryDestroy();
            }
        }

        public void SetLedColor(Color color)
        {
            if (!_ledMat)
            {
                return;
            }

            _ledMat.SetColor(LedColor, color);
        }

        public void SetSpiralLength(float length)
        {
            _spiralMat.SetFloat(Length, length);
        }
    }
}