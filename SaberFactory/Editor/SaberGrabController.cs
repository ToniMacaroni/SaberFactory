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
            if (_isHandleVisisble) return;
            _isHandleVisisble = true;
            _menuPlayerController.leftController?.transform.Find("MenuHandle")?.gameObject.SetActive(_isHandleVisisble);
        }

        public void HideHandle()
        {
            if (!_isHandleVisisble) return;
            _isHandleVisisble = false;
            _menuPlayerController.leftController?.transform.Find("MenuHandle")?.gameObject.SetActive(_isHandleVisisble);
        }
    }
}