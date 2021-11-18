using System;
using System.Collections.Generic;
using HMUI;
using IPA.Utilities;
using SaberFactory.Helpers;
using SiraUtil.Tools;
using UnityEngine;
using UnityEngine.UI;
using VRUIControls;

namespace SaberFactory.UI.Lib
{
    /// <summary>
    ///     Base class to build ui configurations upon
    /// </summary>
    internal class BaseUiComposition
    {
        public GameObject GameObject { get; private set; }

        protected readonly List<CustomScreen> _screens = new List<CustomScreen>();
        protected readonly BsmlDecorator BsmlDecorator;
        protected CurvedCanvasSettings _curvedCanvasSettings;
        protected GameObject _curvedGO;
        private readonly BaseGameUiHandler _baseGameUiHandler;

        private readonly SiraLog _logger;
        private readonly PhysicsRaycasterWithCache _physicsRaycaster;
        private readonly CustomScreen.Factory _screenFactory;

        protected BaseUiComposition(
            SiraLog logger,
            CustomScreen.Factory screenFactory,
            BaseGameUiHandler baseGameUiHandler,
            PhysicsRaycasterWithCache physicsRaycaster,
            BsmlDecorator bsmlDecorator)
        {
            _logger = logger;
            _screenFactory = screenFactory;
            _baseGameUiHandler = baseGameUiHandler;
            _physicsRaycaster = physicsRaycaster;
            BsmlDecorator = bsmlDecorator;
        }

        public event Action OnClosePressed;

        public void Initialize()
        {
            SetupTemplates();

            GameObject = new GameObject(GetType().Namespace + " UI");
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

        protected virtual void SetupUI()
        { }

        public void Open()
        {
            _baseGameUiHandler.DismissGameUI();

            foreach (var screen in _screens)
            {
                screen.Open();
            }

            DidOpen();
        }

        public void Close(bool instant = false)
        {
            foreach (var screen in _screens)
            {
                screen.Close(instant);
            }

            DidClose();

            _baseGameUiHandler.PresentGameUI();
        }

        protected void ClosePressed()
        {
            OnClosePressed?.Invoke();
        }

        protected virtual void DidOpen()
        { }

        protected virtual void DidClose()
        { }

        public void SetRadius(float radius)
        {
            _curvedCanvasSettings.SetRadius(radius);
        }

        protected virtual void SetupTemplates()
        {
            BsmlDecorator.StyleSheetHandler.LoadStyleSheet("SaberFactory.UI.CustomSaber.BaseUiComposition.css");
            BsmlDecorator.AddTemplateHandler("ui-icon", (decorator, args) => "SaberFactory.Resources.UI."+args[0]+".png");
            BsmlDecorator.AddTemplateHandler("icon", (decorator, args) => "SaberFactory.Resources.Icons."+args[0]+".png");
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