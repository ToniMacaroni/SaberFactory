using System.Collections;
using System.Threading.Tasks;
using SiraUtil.Sabers;
using UnityEngine;
using Zenject;

namespace SaberFactory.Game
{
    internal class SaberMovementTester : IInitializable
    {
        private readonly AudioTimeSyncController _audioController;
        private readonly InitData _initData;
        private readonly SiraSaberFactory _saberFactory;

        private Transform _movementContainer;
        private SiraSaber _saber;

        private SaberMovementTester(InitData initData, SiraSaberFactory saberFactory, AudioTimeSyncController audioController)
        {
            _initData = initData;
            _saberFactory = saberFactory;
            _audioController = audioController;
        }

        public async void Initialize()
        {
            await Task.Delay(1000);

            _audioController.Pause();

            if (!_initData.CreateTestingSaber)
            {
                return;
            }

            _movementContainer = new GameObject("SaberTester").transform;
            _movementContainer.localPosition = new Vector3(0, 1.5f, 0);
            _saber = _saberFactory.Spawn(SaberType.SaberA);
            _saber.transform.SetParent(_movementContainer, false);

            SharedCoroutineStarter.instance.StartCoroutine(AnimationCoroutine());
        }

        private IEnumerator AnimationCoroutine()
        {
            var currentRot = _movementContainer.localEulerAngles.x;
            while (true)
            {
                while (currentRot < 90)
                {
                    currentRot += 1;
                    _movementContainer.localEulerAngles = new Vector3(currentRot, 0, 0);
                    yield return new WaitForEndOfFrame();
                }

                while (currentRot > -90)
                {
                    currentRot -= 1;
                    _movementContainer.localEulerAngles = new Vector3(currentRot, 0, 0);
                    yield return new WaitForEndOfFrame();
                }
            }
        }

        internal class InitData
        {
            public bool CreateTestingSaber;
        }
    }
}