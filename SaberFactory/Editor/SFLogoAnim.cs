using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using SaberFactory.Helpers;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SaberFactory.Editor
{
    internal class SFLogoAnim
    {
        private readonly EmbeddedAssetLoader _embeddedAssetLoader;

        private GameObject _instance;
        private Animator _animator;

        public SFLogoAnim(EmbeddedAssetLoader embeddedAssetLoader)
        {
            _embeddedAssetLoader = embeddedAssetLoader;
        }

        public async Task Instantiate(Vector3 pos, Quaternion rot)
        {
            if (_instance) return;
            var prefab = await _embeddedAssetLoader.LoadAsset<GameObject>("SFLogoAnimObject");
            if (!prefab) return;
            _instance = Object.Instantiate(prefab, pos, rot);
            _animator = _instance.GetComponent<Animator>();
            _instance.SetActive(false);
        }

        public async Task PlayAnim()
        {
            if (!_instance) return;
            _instance.SetActive(true);
            _animator.speed = 0.2f;
            _animator.Play("Anim");

            await Task.Delay(2800);

            var scale = 1f;
            while (scale > 0.01f)
            {
                scale -= 0.05f;
                _instance.transform.localScale = new Vector3(scale, scale, scale);
                await Task.Delay(10);
            }

            Destroy();
        }

        public void Destroy()
        {
            _instance.TryDestroy();
        }
    }
}