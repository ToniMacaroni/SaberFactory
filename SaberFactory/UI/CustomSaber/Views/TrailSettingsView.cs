using System.Linq;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components.Settings;
using HMUI;
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
using Zenject;


namespace SaberFactory.UI.CustomSaber.Views
{
    internal class TrailSettingsView : SubView, INavigationCategoryView
    {
        public ENavigationCategory Category => ENavigationCategory.Trail;

        [Inject] private readonly TrailPreviewer _trailPreviewer = null;
        [Inject] private readonly EditorInstanceManager _editorInstanceManager = null;
        [Inject] private readonly PlayerDataModel _playerDataModel = null;
        [Inject] private readonly MainAssetStore _mainAssetStore = null;
        [Inject] private readonly PluginConfig _pluginConfig = null;
        [Inject] private readonly TrailConfig _trailConfig = null;

        private InstanceTrailData _instanceTrailData;

        [UIObject("main-container")] private readonly GameObject _mainContainer = null;
        [UIObject("no-trail-container")] private readonly GameObject _noTrailContainer = null;
        [UIObject("advanced-container")] private readonly GameObject _advancedContainer = null;

        [UIComponent("length-slider")] private readonly SliderSetting _lengthSliderSetting = null;
        [UIComponent("width-slider")] private readonly SliderSetting _widthSliderSetting = null;
        [UIComponent("whitestep-slider")] private readonly SliderSetting _whitestepSliderSetting = null;
        [UIComponent("offset-slider")] private readonly SliderSetting _offsetSliderSetting = null;
        [UIComponent("clamp-checkbox")] private readonly ToggleSetting _clampToggleSetting = null;
        [UIComponent("flip-checkbox")] private readonly ToggleSetting _flipToggleSetting = null;

        [UIComponent("material-editor")] private readonly MaterialEditor _materialEditor = null;
        [UIComponent("choose-trail-popup")] private readonly ChooseTrailPopup _chooseTrailPopup = null;

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
            set => _trailConfig.OnlyUseVertexColor = value;
        }

        private SliderController _lengthSlider;
        private SliderController _widthSlider;
        private SliderController _whitestepSlider;
        private SliderController _offsetSlider;
        private ToggleController _clampToggle;
        private ToggleController _flipToggle;
        private bool _refreshButtonActive;

        private bool _autoUpdateTrail;
        private bool _dirty;
        private float _time;

        [UIAction("#post-parse")]
        private void Setup()
        {
            _autoUpdateTrail = _pluginConfig.AutoUpdateTrail;

            _lengthSlider = new SliderController(_lengthSliderSetting);
            _widthSlider = new SliderController(_widthSliderSetting);
            _whitestepSlider = new SliderController(_whitestepSliderSetting);
            _offsetSlider = new SliderController(_offsetSliderSetting);
            _clampToggle = new ToggleController(_clampToggleSetting);
            _flipToggle = new ToggleController(_flipToggleSetting);

            _mainContainer.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            _advancedContainer.SetActive(_pluginConfig.ShowAdvancedTrailSettings);
        }

        public override void DidOpen()
        {
            CreateTrail(_editorInstanceManager.CurrentSaber);

            _editorInstanceManager.OnSaberInstanceCreated += CreateTrail;
        }

        public override void DidClose()
        {
            _materialEditor.Close();
            _instanceTrailData = null;
            _trailPreviewer.Destroy();

            _editorInstanceManager.OnSaberInstanceCreated -= CreateTrail;
        }

        private void LoadFromModel(InstanceTrailData trailData)
        {
            _instanceTrailData = trailData;

            _lengthSlider.Value = _instanceTrailData.Length;
            _widthSlider.Value = _instanceTrailData.Width;
            _whitestepSlider.Value = _instanceTrailData.WhiteStep;
            _offsetSlider.Value = _instanceTrailData.Offset;
            _clampToggle.Value = _instanceTrailData.ClampTexture;
            _flipToggle.Value = _instanceTrailData.Flip;
        }

        private void SetLength(RangeValuesTextSlider slider, float val)
        {
            _instanceTrailData.Length = (int) val;
            _dirty = true;
            if (_refreshButtonActive)
            {
                return;
            }
            _trailPreviewer.SetLength(val);
        }

        private void SetWidth(RangeValuesTextSlider slider, float val)
        {
            _instanceTrailData.Width = val;
            if (_refreshButtonActive)
            {
                return;
            }
            _trailPreviewer.UpdateWidth();
        }

        private void SetOffset(RangeValuesTextSlider slider, float val)
        {
            _instanceTrailData.Offset = val;
            if (_refreshButtonActive) return;
            _trailPreviewer.UpdateWidth();
        }

        private void SetWhitestep(RangeValuesTextSlider slider, float val)
        {
            _instanceTrailData.WhiteStep = val;
        }

        private void SetClampMode(bool shouldClamp)
        {
            _instanceTrailData.ClampTexture = shouldClamp;
        }

        private void SetFlip(bool flip)
        {
            _instanceTrailData.Flip = flip;
            if (_refreshButtonActive) RefreshTrail();
            else CreateTrail(_editorInstanceManager.CurrentSaber);
        }

        private void SetTrailModel(TrailModel trailModel)
        {
            if (_editorInstanceManager.CurrentPiece is CustomSaberInstance customsaber)
            {
                var model = (CustomSaberModel)customsaber.Model;
                model.TrailModel = trailModel;
            }
        }

        private void CopyFromTrailModel(TrailModel trailModel)
        {
            if (_editorInstanceManager.CurrentPiece is CustomSaberInstance customsaber)
            {
                var model = (CustomSaberModel)customsaber.Model;
                model.TrailModel.CopyFrom(trailModel);
            }
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

        private void AddControlEvents()
        {
            _lengthSlider.AddEvent(SetLength);
            _widthSlider.AddEvent(SetWidth);
            _whitestepSlider.AddEvent(SetWhitestep);
            _offsetSlider.AddEvent(SetOffset);
            _clampToggle.SetEvent(SetClampMode);
            _flipToggle.SetEvent(SetFlip);
        }

        private void RemoveControlEvents()
        {
            _lengthSlider.RemoveEvent();
            _widthSlider.RemoveEvent();
            _whitestepSlider.RemoveEvent();
            _offsetSlider.RemoveEvent();
            _clampToggle.RemoveEvent();
            _flipToggle.RemoveEvent();
        }

        private void CreateTrail(SaberInstance saberInstance)
        {
            _dirty = false;
            _trailPreviewer.Destroy();

            RemoveControlEvents();

            var trailData = saberInstance?.GetTrailData(out _);

            // Show no trail container and return
            if (trailData is null)
            {
                if(_mainContainer.activeSelf) _mainContainer.SetActive(false);
                if(!_noTrailContainer.activeSelf) _noTrailContainer.SetActive(true);
                return;
            }

            // Show main container in case it wasn't active
            if (_noTrailContainer.activeSelf) _noTrailContainer.SetActive(false);
            if (!_mainContainer.activeSelf) _mainContainer.SetActive(true);

            if (saberInstance.TrailHandler is {})
            {
                CreateTrailHand(trailData);
                RefreshButtonActive = true;
            }
            else
            {
                CreateTrailPedestal(saberInstance, trailData);
                RefreshButtonActive = false;
            }

            AddControlEvents();
        }

        private void CreateTrailPedestal(SaberInstance saberInstance, InstanceTrailData trailData)
        {
            _trailPreviewer.Create(saberInstance.GameObject.transform.parent, trailData);

            LoadFromModel(trailData);

            _trailPreviewer.SetColor(_playerDataModel.playerData.colorSchemesSettings.GetSelectedColorScheme().saberAColor);
        }

        private void CreateTrailHand(InstanceTrailData trailData)
        {
            LoadFromModel(trailData);

            AddControlEvents();
        }

        private void TrailPopupSelectionChanged(TrailModel trailModel)
        {
            if (trailModel is null)
            {
                ResetTrail();
            }
            else
            {
                CopyFromTrailModel(trailModel);
            }

            _editorInstanceManager.Refresh();
        }

        private void Update()
        {
            if (!_autoUpdateTrail) return;

            if (_time < 0.3)
            {
                _time += Time.deltaTime;
                return;
            }

            _time = 0;

            if (!_dirty || _instanceTrailData == null || !_refreshButtonActive) return;
            _dirty = false;

            RefreshTrail();
        }

        private void UpdateProps()
        {
            ParserParams.EmitEvent("update-props");
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
            _editorInstanceManager.CurrentSaber.TrailHandler?.SetColor(_playerDataModel.playerData.colorSchemesSettings.GetSelectedColorScheme().saberAColor);
        }

        [UIAction("revert-advanced")]
        private void RevertAdvanced()
        {
            _trailConfig.Revert();
            ParserParams.EmitEvent("get-advanced");
        }
    }
}
