using SaberFactory.Instances;
using UnityEngine;

namespace SaberFactory.Editor
{
    internal class SaberGrabController
    {
        public readonly Transform GrabContainer;
        private readonly MenuPlayerController _menuPlayerController;
        private SaberInstance _currentSaberInstancce;

        private bool _isHandleVisisble = true;

        public SaberGrabController(MenuPlayerController menuPlayerController)
        {
            _menuPlayerController = menuPlayerController;
            GrabContainer = new GameObject("SaberGrabContainer").transform;
            GrabContainer.SetParent(menuPlayerController.leftController.transform, false);
        }

        public void GrabSaber(SaberInstance saberInstance)
        {
            HideHandle();
            _currentSaberInstancce = saberInstance;
            saberInstance.SetParent(GrabContainer);
        }

        public void ShowHandle()
        {
            if (_isHandleVisisble)
            {
                return;
            }

            _isHandleVisisble = true;

            if (_menuPlayerController.leftController != null && _menuPlayerController.leftController.transform.Find("MenuHandle") is { } handle)
            {
                handle.gameObject.SetActive(_isHandleVisisble);
            }
        }

        public void HideHandle()
        {
            if (_menuPlayerController.leftController.transform.Find("LeftSaber") is { } leftSaber)
            {
                leftSaber.gameObject.SetActive(false);
            }

            if (_menuPlayerController.leftController.transform.Find("LeftTrail") is { } leftTrail)
            {
                leftTrail.gameObject.SetActive(false);
            }

            //var cam1 = GameObject.Find("Cam2_Side View").transform;
            //var cam2 = GameObject.Find("Cam2_Side View2").transform;

            //cam1.parent = _menuPlayerController.leftController.transform;
            //cam2.parent = _menuPlayerController.rightController.transform;

            //cam1.localPosition = new Vector3(0.48f, -0.95f, 0);
            //cam2.localPosition = new Vector3(0.48f, -0.95f, 0);

            //cam1.localEulerAngles = new Vector3(0, 0, 0);
            //cam2.localEulerAngles = new Vector3(0, 0, 0);

            if (!_isHandleVisisble)
            {
                return;
            }

            _isHandleVisisble = false;

            if (_menuPlayerController != null && _menuPlayerController.leftController.transform.Find("MenuHandle") is { } handle)
            {
                handle.gameObject.SetActive(_isHandleVisisble);
            }
        }
    }
}