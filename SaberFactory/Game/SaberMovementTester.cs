using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using SiraUtil.Sabers;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace SaberFactory.Game
{
    internal class SaberMovementTester : IInitializable
    {
        private readonly AudioTimeSyncController _audioController;
        private readonly InitData _initData;
        private readonly SiraSaberFactory _saberFactory;
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

            var saberA = CreateSaber(SaberType.SaberA, new Vector3(0, 0.6f, 0), Quaternion.Euler(90, 0, 0));
            var saberB = CreateSaber(SaberType.SaberB, new Vector3(0, 0.6f, 0), Quaternion.Euler(90, 0, 0));

            SharedCoroutineStarter.instance.StartCoroutine(GroundRoundAnimationCoroutine(-0.2f, saberA));
            SharedCoroutineStarter.instance.StartCoroutine(GroundRoundAnimationCoroutine(0.2f, saberB));

            // Don't try this at home
            var allLRs = Object.FindObjectsOfType<LineRenderer>()
                .Where(x => x.name.Contains("SaberBurnMark"));

            var normalLR = allLRs.First(x => !x.name.Contains("Sira"));
            var siraLRs = allLRs.Where(x => x.name.Contains("Sira"));

            foreach (var lineRenderer in siraLRs)
            {
                lineRenderer.sharedMaterial = new Material(normalLR.sharedMaterial);
                lineRenderer.textureMode = LineTextureMode.Stretch;
                //lineRenderer.widthMultiplier = 2;
                Console.WriteLine("Replaced mat");
            }
        }

        public Transform CreateSaber(SaberType saberType, Vector3 pos, Quaternion rot)
        {
            var parent = new GameObject("SaberTester_" + saberType).transform;
            parent.localPosition = new Vector3(0, 0.6f, 0);
            parent.localRotation = Quaternion.Euler(90, 0, 0);
            _saber = _saberFactory.Spawn(saberType);
            _saber.transform.SetParent(parent, false);

            return parent;
        }

        //private IEnumerator AnimationCoroutine()
        //{
        //    var currentRot = _movementContainer.localEulerAngles.x;
        //    while (true)
        //    {
        //        while (currentRot < 90)
        //        {
        //            currentRot += 1;
        //            _movementContainer.localEulerAngles = new Vector3(currentRot, 0, 0);
        //            yield return new WaitForEndOfFrame();
        //        }

        //        while (currentRot > -90)
        //        {
        //            currentRot -= 1;
        //            _movementContainer.localEulerAngles = new Vector3(currentRot, 0, 0);
        //            yield return new WaitForEndOfFrame();
        //        }
        //    }
        //}

        private IEnumerator GroundRoundAnimationCoroutine(float xPos, Transform t)
        {
            var currentPos = t.localPosition.z;
            while (true)
            {
                while (currentPos < 1)
                {
                    currentPos += 0.02f;
                    t.localPosition = new Vector3(xPos, 0.6f, currentPos);
                    yield return new WaitForEndOfFrame();
                }

                while (currentPos > 0)
                {
                    currentPos -= 0.02f;
                    t.localPosition = new Vector3(xPos, 0.6f, currentPos);
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