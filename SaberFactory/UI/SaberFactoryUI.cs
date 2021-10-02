using System;
using System.Collections.Generic;
using HMUI;
using IPA.Utilities;
using SaberFactory.Helpers;
using SaberFactory.UI.Lib;
using SiraUtil.Tools;
using UnityEngine;
using UnityEngine.UI;
using VRUIControls;

namespace SaberFactory.UI
{
    /// <summary>
    ///     Base class to build ui configurations upon
    /// </summary>
    internal class SaberFactoryUI
    {
        public GameObject GameObject { get; private set; }
        protected CurvedCanvasSettings _curvedCanvasSettings;
        protected GameObject _curvedGO;

        protected List<CustomScreen> _screens;
        private readonly BaseGameUiHandler _baseGameUiHandler;

        private readonly SiraLog _logger;
        private readonly PhysicsRaycasterWithCache _physicsRaycaster;
        private readonly CustomScreen.Factory _screenFactory;

        protected SaberFactoryUI(SiraLog logger, CustomScreen.Factory screenFactory, BaseGameUiHandler baseGameUiHandler,
            PhysicsRaycasterWithCache physicsRaycaster)
        {
            _logger = logger;
            _screenFactory = screenFactory;
            _baseGameUiHandler = baseGameUiHandler;
            _physicsRaycaster = physicsRaycaster;

            _screens = new List<CustomScreen>();
        }

        public event Action OnClosePressed;

        public void Initialize()
        {
            GameObject = new GameObject("Saber Factory UI");
            GameObject.transform.SetParent(_baseGameUiHandler.GetUIParent(), false);
            GameObject.transform.localPosition = new Vector3(0, 1.1f, 2.6f);
            GameObject.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);

            _curvedGO = GameObject.CreateGameObject("Curved UI");
            _curvedGO.AddComponent<Canvas>().sortingOrder = 3;

            var canvasScaler = _curvedGO.AddComponent<CanvasScaler>();
            canvasScaler.referencePixelsPerUnit = 10;
            canvasScaler.scaleFactor = 3.44f;

            var vrgr = _curvedGO.AddComponent<VRGraphicRaycaster>();
            vrgr.SetField("_physicsRaycaster", _physicsRaycaster);

            _curvedGO.AddComponent<CanvasRenderer>();
            _curvedCanvasSettings = _curvedGO.AddComponent<CurvedCanvasSettings>();

            SetupUI();
        }

        public virtual void SetupUI()
        {
        }

        public void Open()
        {
            _baseGameUiHandler.DismissGameUI();

            foreach (var screen in _screens) screen.Open();

            DidOpen();
        }

        public void Close(bool instant = false)
        {
            foreach (var screen in _screens) screen.Close(instant);

            DidClose();

            _baseGameUiHandler.PresentGameUI();
        }

        protected void ClosePressed()
        {
            OnClosePressed?.Invoke();
        }

        protected virtual void DidOpen()
        {
        }

        protected virtual void DidClose()
        {
        }

        public void SetRadius(float radius)
        {
            _curvedCanvasSettings.SetRadius(radius);
        }

        protected CustomScreen AddScreen(CustomScreen.InitData initData)
        {
            initData.Parent = initData.IsCurved ? _curvedGO.transform : GameObject.transform;
            var screen = _screenFactory.Create(initData);
            _screens.Add(screen);
            return screen;
        }
    }
}