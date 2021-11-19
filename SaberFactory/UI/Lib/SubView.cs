using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage.Parser;
using HMUI;
using JetBrains.Annotations;
using SaberFactory.Helpers;
using SiraUtil.Tools;
using UnityEngine;
using UnityEngine.UI;
using VRUIControls;
using Zenject;

namespace SaberFactory.UI.Lib
{
    internal class SubView : MonoBehaviour, INotifyPropertyChanged
    {
        public virtual bool IsActive => gameObject.activeSelf;

        protected BSMLParserParams ParserParams { get; private set; }

        protected virtual string _resourceName => string.Join(".", GetType().Namespace, GetType().Name);

        protected bool IsPatView => _resourceName.EndsWith("PatreonView.bsml");

        protected const string PatViewPath = "SaberFactory.UI.PatreonView.bsml";

        public SubViewSwitcher SubViewSwitcher;

        protected SiraLog Logger;
        private BsmlDecorator _bsmlDecorator;
        private bool _firstActivation = true;
        private PhysicsRaycasterWithCache _raycasterWithCache;

        public event PropertyChangedEventHandler PropertyChanged;

        [Inject]
        private void Construct(SiraLog logger, BsmlDecorator bsmlDecorator, PhysicsRaycasterWithCache raycasterWithCache)
        {
            _bsmlDecorator = bsmlDecorator;
            Logger = logger;
            _raycasterWithCache = raycasterWithCache;
        }

        public async Task Open(bool notify = true)
        {
            if (_firstActivation)
            {
                ParserParams = await _bsmlDecorator.ParseFromResourceAsync(_resourceName, gameObject, this);

                gameObject.SetActive(false);
                _firstActivation = false;
                
                foreach (var obj in ParserParams.GetObjectsWithTag("canvas"))
                {
                    var newParent = obj.transform.parent.CreateGameObject("CanvasContainer");
                    newParent.AddComponent<RectTransform>();
                    newParent.AddComponent<VerticalLayoutGroup>();
                    newParent.AddComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                    newParent.AddComponent<LayoutElement>();
                
                    newParent.AddComponent<Canvas>();
                    var canvasScaler = newParent.AddComponent<CanvasScaler>();
                    canvasScaler.referencePixelsPerUnit = 10;
                    canvasScaler.scaleFactor = 3.44f;

                    newParent.AddComponent<CurvedCanvasSettings>();
                    UIHelpers.AddVrRaycaster(newParent, _raycasterWithCache);
                
                    obj.transform.SetParent(newParent.transform, false);
                }
                
                await Init();
            }

            gameObject.SetActive(true);
            if (notify)
            {
                DidOpen();
            }
        }

        public void Close()
        {
            DidClose();
        }

        public virtual void DidOpen()
        { }

        public virtual void DidClose()
        { }

        public void GoBack()
        {
            SubViewSwitcher.GoBack();
        }

        public void UpdateProps()
        {
            ParserParams.EmitEvent("update-props");
        }

        protected virtual Task Init()
        {
            return Task.CompletedTask;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        internal class Factory : PlaceholderFactory<Type, InitData, SubView>
        { }

        internal struct InitData
        {
            public string Name;
            public Transform Parent;
        }
    }
}