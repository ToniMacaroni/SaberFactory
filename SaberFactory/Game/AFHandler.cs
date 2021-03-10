using System;
using System.Threading.Tasks;
using IPA.Utilities;
using Tweening;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SaberFactory.Game
{
    internal class AFHandler : IDisposable
    {
        private readonly EmbeddedAssetLoader _assetLoader;
        public static bool ShouldFire = true;

        public static bool IsValid
        {
            get
            {
                if (!_isValid.HasValue)
                {
                    var time = Utils.CanUseDateTimeNowSafely ? DateTime.Now : DateTime.UtcNow;
                    _isValid = (time.Month == 4 && time.Day == 1);
                }

                return _isValid.Value;
            }
        }

        private static bool? _isValid;
        private bool _hasFired;
        private GameObject _thruster;

        private AFHandler(EmbeddedAssetLoader assetLoader)
        {
            _assetLoader = assetLoader;
        }

        public async Task Shoot(SFSaberModelController smc, SaberType saberType)
        {
            _hasFired = true;
            await InitPosition(smc.transform, saberType==SaberType.SaberA?-0.1f:0.1f);
            await Task.Delay(500);

            var thruster = Object.Instantiate(await GetThruster());
            thruster.transform.SetParent(smc.transform, false);
            thruster.transform.localEulerAngles = new Vector3(-90, 0, 0);
            thruster.transform.localPosition = new Vector3(0, 0, -0.2f);

            var rigidBody = smc.gameObject.AddComponent<Rigidbody>();
            rigidBody.useGravity = false;
            rigidBody.AddForce(Quaternion.Euler(-10, 0, 0)*new Vector3(0, 0, 100));
        }

        public async Task InitPosition(Transform transform, float xPos)
        {
            transform.SetParent(null, true);

            var posTween = new Vector3Tween(transform.position, new Vector3(xPos, 1, 1), pos =>
            {
                transform.position = pos;
            }, 1, EaseType.OutCubic);

            var rotTween = new Vector3Tween(transform.eulerAngles, new Vector3(-10, 0, 0), pos =>
            {
                transform.eulerAngles = pos;
            }, 1, EaseType.OutCubic);

            var t = 0f;
            while (t < 1)
            {
                t += 0.01f;
                posTween.Sample(t);
                rotTween.Sample(t);
                await Task.Delay(10);
            }
        }

        public void Dispose()
        {
            ShouldFire = !_hasFired;
        }

        private async Task<GameObject> GetThruster()
        {
            return _thruster ??= await _assetLoader.LoadAsset<GameObject>("Thruster");
        }
    }
}