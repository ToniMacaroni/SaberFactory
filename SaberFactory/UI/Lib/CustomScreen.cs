using HMUI;
using SaberFactory.Helpers;
using SiraUtil.Logging;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Screen = HMUI.Screen;

namespace SaberFactory.UI.Lib
{
    /// <summary>
    ///     Implementation of a <see cref="HMUI.Screen" />
    ///     basically a container for viewcontrollers
    /// </summary>
    internal class CustomScreen : Screen
    {
        public ViewController CurrentViewController { get; private set; }
        private SiraLog _logger;
        private CustomViewController.Factory _viewControllerFactory;

        [Inject]
        private void Construct(SiraLog logger, CustomViewController.Factory viewControllerFactory)
        {
            _logger = logger;
            _viewControllerFactory = viewControllerFactory;
        }

        public void Initialize(InitData initData)
        {
            var t = gameObject.transform;
            t.localPosition = initData.Position;

            var canvas = gameObject.AddComponent<Canvas>();

            canvas.transform.AsRectTransform().sizeDelta = initData.Size;
            gameObject.AddComponent<CanvasRenderer>();

            if (!initData.IsCurved)
            {
                var canvasScaler = gameObject.AddComponent<CanvasScaler>();
                canvasScaler.referencePixelsPerUnit = 10;
                canvasScaler.scaleFactor = 3.44f;

                var curvedCanvasSettings = gameObject.AddComponent<CurvedCanvasSettings>();
                curvedCanvasSettings.SetRadius(initData.CurveRadius);
            }
        }

        public T CreateViewController<T>() where T : CustomViewController
        {
            var initData = new CustomViewController.InitData
            {
                Parent = gameObject.transform,
                Screen = this
            };

            CurrentViewController = _viewControllerFactory.Create(typeof(T), initData);
            return (T)CurrentViewController;
        }

        public virtual async void Open()
        {
            SetRootViewController(CurrentViewController, ViewController.AnimationType.In);
            await CurrentViewController.Cast<CustomViewController>().AnimateIn(CancellationToken.None);
        }

        public virtual void Close(bool instant = false)
        {
            SetRootViewController(null, ViewController.AnimationType.Out);
            if (instant) gameObject.SetActive(false);
        }

        internal class Factory : PlaceholderFactory<InitData, CustomScreen>
        {
        }

        internal struct InitData
        {
            public string Name;
            public Vector3 Position;
            public Quaternion Rotation;
            public Vector2 Size;
            public bool IsCurved;
            public float CurveRadius;
            public Transform Parent;

            public InitData(string name, Vector3 position, Quaternion rotation, Vector2 size, bool isCurved) : this()
            {
                Name = name;
                Position = position;
                Rotation = rotation;
                Size = size;
                IsCurved = isCurved;
            }
        }
    }
}