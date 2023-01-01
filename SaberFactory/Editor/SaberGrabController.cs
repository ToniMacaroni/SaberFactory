using SaberFactory.Instances;
using UnityEngine;

namespace SaberFactory.Editor
{
    internal class SaberGrabController : ISaberPresenter
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

        public void Present(SaberInstance saberInstance)
        {
            HideHandle();
            _currentSaberInstancce = saberInstance;
            saberInstance.CreateTrail(true);
            saberInstance.SetParent(GrabContainer);
        }

        public void StopPresenting()
        {
            ShowHandle();
        }
    }
}