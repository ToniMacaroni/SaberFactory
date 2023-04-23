using System.Collections.Generic;
using System.Linq;
using BeatSaberMarkupLanguage.Attributes;
using CustomSaber;
using SaberFactory.Configuration;
using SaberFactory.DataStore;
using SaberFactory.Editor;
using SaberFactory.Helpers;
using SaberFactory.Instances;
using SaberFactory.Instances.CustomSaber;
using SaberFactory.Instances.Trail;
using SaberFactory.Models;
using SaberFactory.Models.CustomSaber;
using SaberFactory.UI.CustomSaber.CustomComponents;
using SaberFactory.UI.CustomSaber.Popups;
using SaberFactory.UI.Lib;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Zenject;

namespace SaberFactory.UI.CustomSaber.Views
{
    internal class TrailSettingsView : SubView, INavigationCategoryView
    {
        [UIObject("advanced-container")] private readonly GameObject _advancedContainer = null;
        [UIComponent("choose-trail-popup")] private readonly ChooseTrailPopup _chooseTrailPopup = null;

        [UIObject("main-container")] private readonly GameObject _mainContainer = null;
        [UIObject("no-trail-container")] private readonly GameObject _noTrailContainer = null;

        [UIComponent("material-editor")] private readonly MaterialEditor _materialEditor = null;

        [UIValue("trail-width-max")] private float _trailWidthMax => _pluginConfig.TrailWidthMax;

        [UIValue("granularity-value")]
        private int GranularityValue
        {
            get => _trailConfig.Granularity;
            set => _trailConfig.Granularity = value;
        }

        [UIValue("sampling-frequency-value")]
        private int SamplingFrequencyValue
        {
            get => _trailConfig.SamplingFrequency;
            set => _trailConfig.SamplingFrequency = value;
        }

        [UIValue("refresh-button-active")]
        private bool RefreshButtonActive
        {
            get => _refreshButtonActive;
            set
            {
                _refreshButtonActive = value;
                OnPropertyChanged();
            }
        }

        private bool UseVertexColorOnly
        {
            get => _trailConfig.OnlyUseVertexColor;
            set
            {
                _trailConfig.OnlyUseVertexColor = value;
                _trailPreviewer.OnlyColorVertex = value;
            }
        }

        private bool NoTrailViewActive
        {
            get => _noTrailContainer.activeSelf;
            set
            {
                if (_noTrailContainer.activeSelf == !value)
                {
                    _noTrailContainer.SetActive(value);
                }

                if (_mainContainer.activeSelf == value)
                {
                    _mainContainer.SetActive(!value);
                }
            }
        }

        private bool ShowThumbstickMessage => _pluginConfig.ControlTrailWithThumbstick;
        [Inject] private readonly EditorInstanceManager _editorInstanceManager = null;
        [Inject] private readonly MainAssetStore _mainAssetStore = null;
        [Inject] private readonly PlayerDataModel _playerDataModel = null;
        [Inject] private readonly PluginConfig _pluginConfig = null;
        [Inject] private readonly TrailConfig _trailConfig = null;

        [Inject] private readonly TrailPreviewer _trailPreviewer = null;
        [Inject] private readonly IVRPlatformHelper _vrPlatformHelper = null;

        private bool _dirty;

        private InstanceTrailData _instanceTrailData;

        private bool _refreshButtonActive;
        private float _time;
        private float _trailFloatLength;

        public ENavigationCategory Category => ENavigationCategory.Trail;

        private void Update()
        {
            if (_time < 0.3)
            {
                _time += Time.deltaTime;
                return;
            }

            _time = 0;

            if (!_dirty || _instanceTrailData == null || !_refreshButtonActive)
            {
                return;
            }

            _dirty = false;

            RefreshTrail();
        }

        [UIAction("#post-parse")]
        private void Setup()
        {
            _mainContainer.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            _advancedContainer.SetActive(_pluginConfig.ShowAdvancedTrailSettings);
        }

        public override void DidOpen()
        {
            CreateTrail(_editorInstanceManager.CurrentSaber);

            _editorInstanceManager.OnSaberInstanceCreated += CreateTrail;

            // if (_instanceTrailData != null && _pluginConfig.ControlTrailWithThumbstick)
            // {
            //     _trailFloatLength = _instanceTrailData.Length;
            //     _vrPlatformHelper.joystickWasNotCenteredThisFrameEvent += OnjoystickWasNotCenteredThisFrameEvent;
            // }
        }

        public override void DidClose()
        {
            // if (_instanceTrailData != null && _pluginConfig.ControlTrailWithThumbstick)
            // {
            //     _vrPlatformHelper.joystickWasNotCenteredThisFrameEvent -= OnjoystickWasNotCenteredThisFrameEvent;
            // }

            _editorInstanceManager.OnSaberInstanceCreated -= CreateTrail;

            _materialEditor.Close();
            _chooseTrailPopup.Exit();
            _instanceTrailData = null;
            _trailPreviewer.Destroy();
        }

        private void OnjoystickWasNotCenteredThisFrameEvent(Vector2 deltaPos)
        {
            WidthValue = Mathf.Clamp(_instanceTrailData.Width + deltaPos.y * -0.005f, 0, _trailWidthMax);

            LengthValue = Mathf.Clamp(_trailFloatLength + deltaPos.x * 0.1f, 0, 30);

            ParserParams.EmitEvent("update-proportions");
        }

        private void LoadFromModel(InstanceTrailData trailData)
        {
            _instanceTrailData = trailData;

            _trailFloatLength = _instanceTrailData?.Length ?? 0;
            UpdateProps();
        }

        private void SetTrailModel(TrailModel trailModel)
        {
            if (_editorInstanceManager.CurrentPiece is CustomSaberInstance customsaber)
            {
                var model = (CustomSaberModel)customsaber.Model;
                model.TrailModel = trailModel;
            }
        }

        private bool CopyFromTrailModel(TrailModel trailModel, List<CustomTrail> trailList)
        {
            if (_editorInstanceManager.CurrentPiece is CustomSaberInstance customsaber)
            {
                var model = (CustomSaberModel)customsaber.Model;

                if (model.TrailModel == null)
                {
                    model.TrailModel = new TrailModel(
                        Vector3.zero,
                        0.5f,
                        12,
                        new MaterialDescriptor(null),
                        0f,
                        TextureWrapMode.Clamp) { TrailOriginTrails = trailList };

                    model.TrailModel.CopyFrom(trailModel);
                    model.TrailModel.Material.UpdateBackupMaterial(false);

                    return true;
                }

                model.TrailModel.CopyFrom(trailModel);
                model.TrailModel.TrailOriginTrails = trailList;
            }

            return false;
        }

        private void ResetTrail()
        {
            if (_editorInstanceManager.CurrentPiece is CustomSaberInstance cs)
            {
                _instanceTrailData.RevertMaterialForCustomSaber(cs.Model as CustomSaberModel);
                var tm = _editorInstanceManager.CurrentModelComposition?.GetLeft().CastChecked<CustomSaberModel>()?.GrabTrail(false);
                if (tm is { })
                {
                    SetTrailModel(tm);
                }
            }
        }

        private void CreateTrail(SaberInstance saberInstance)
        {
            _dirty = false;
            _trailPreviewer.Destroy();

            var trailData = saberInstance?.GetTrailData(out _);

            // Show "no trail" container and return
            if (trailData is null)
            {
                NoTrailViewActive = true;
                return;
            }

            // Show main container in case it wasn't active
            NoTrailViewActive = false;

            if (saberInstance.TrailHandler is { })
            {
                //in hand
                LoadFromModel(trailData);
                RefreshButtonActive = true;
            }
            else
            {
                //on pedestal
                _trailPreviewer.Create(saberInstance.GameObject.transform.parent, trailData, UseVertexColorOnly);
                LoadFromModel(trailData);
                _trailPreviewer.SetColor(_playerDataModel.playerData.colorSchemesSettings.GetSelectedColorScheme().saberAColor);
                RefreshButtonActive = false;
            }
        }

        private void TrailPopupSelectionChanged(TrailModel trailModel, List<CustomTrail> trailList)
        {
            if (trailModel is null)
            {
                ResetTrail();
            }
            else
            {
                CopyFromTrailModel(trailModel, trailList);
            }

            _editorInstanceManager.Refresh();
        }

        [UIAction("edit-material")]
        private void EditMaterial()
        {
            _materialEditor.Show(_instanceTrailData.Material);
        }

        [UIAction("revert-trail")]
        private void ClickRevertTrail()
        {
            ResetTrail();
            _editorInstanceManager.Refresh();
        }

        [UIAction("choose-trail")]
        private void ClickChooseTrail()
        {
            _chooseTrailPopup.Show(
                from meta in _mainAssetStore.GetAllMetaData(AssetTypeDefinition.CustomSaber)
                orderby meta.IsFavorite descending
                select meta,
                TrailPopupSelectionChanged);
        }

        [UIAction("refresh-trail")]
        private void RefreshTrail()
        {
            _editorInstanceManager.CurrentSaber.DestroyTrail(true);
            _editorInstanceManager.CurrentSaber.CreateTrail(true);
            _editorInstanceManager.CurrentSaber.TrailHandler?.SetColor(_playerDataModel.playerData.colorSchemesSettings.GetSelectedColorScheme()
                .saberAColor);
        }

        [UIAction("revert-advanced")]
        private void RevertAdvanced()
        {
            _trailConfig.Revert();
            ParserParams.EmitEvent("get-advanced");
        }

        #region Values

        private float LengthValue
        {
            get => _trailFloatLength;
            set
            {
                _trailFloatLength = value;
                if (_instanceTrailData is null)
                {
                    return;
                }

                _instanceTrailData.Length = (int)value;
                _dirty = true;
                if (_refreshButtonActive)
                {
                    return;
                }

                _trailPreviewer.SetLength(value);
                OnPropertyChanged();
            }
        }

        private float WidthValue
        {
            get => _instanceTrailData?.Width ?? 1;
            set
            {
                if (_instanceTrailData is null)
                {
                    return;
                }

                _instanceTrailData.Width = value;
                if (_refreshButtonActive)
                {
                    return;
                }

                _trailPreviewer.UpdateWidth();
                OnPropertyChanged();
            }
        }

        private float OffsetValue
        {
            get => _instanceTrailData?.Offset ?? 0;
            set
            {
                if (_instanceTrailData is null)
                {
                    return;
                }

                _instanceTrailData.Offset = value;
                if (_refreshButtonActive)
                {
                    return;
                }

                _trailPreviewer.UpdateWidth();
                OnPropertyChanged();
            }
        }

        private float WhitestepValue
        {
            get => _instanceTrailData?.WhiteStep ?? 0;
            set
            {
                if (_instanceTrailData is null)
                {
                    return;
                }

                _instanceTrailData.WhiteStep = value;
                OnPropertyChanged();
            }
        }

        private bool ClampValue
        {
            get => _instanceTrailData?.ClampTexture ?? false;
            set
            {
                if (_instanceTrailData is null)
                {
                    return;
                }

                _instanceTrailData.ClampTexture = value;
                OnPropertyChanged();
            }
        }

        private bool FlipValue
        {
            get => _instanceTrailData?.Flip ?? false;
            set
            {
                if (_instanceTrailData is null)
                {
                    return;
                }

                _instanceTrailData.Flip = value;
                if (_refreshButtonActive)
                {
                    RefreshTrail();
                }
                else
                {
                    CreateTrail(_editorInstanceManager.CurrentSaber);
                }

                OnPropertyChanged();
            }
        }

        #endregion
    }
}