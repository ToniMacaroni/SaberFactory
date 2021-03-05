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
using SaberFactory.UI.Lib;
using UnityEngine;
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
        [UIComponent("clamp-checkbox")] private readonly ToggleSetting _clampToggleSetting = null;

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

        private SliderController _lengthSlider;
        private SliderController _widthSlider;
        private SliderController _whitestepSlider;
        private ToggleController _clampToggle;
        private bool _refreshButtonActive;

        [UIAction("#post-parse")]
        private void Setup()
        {
            _lengthSlider = new SliderController(_lengthSliderSetting);
            _widthSlider = new SliderController(_widthSliderSetting);
            _whitestepSlider = new SliderController(_whitestepSliderSetting);
            _clampToggle = new ToggleController(_clampToggleSetting);

            if (_pluginConfig.ShowAdvancedTrailSettings)
            {
                var rect = _mainContainer.GetComponent<RectTransform>();
                var size = rect.sizeDelta;
                size.y = -55;
                rect.sizeDelta = size;
            }
            else
            {
                _advancedContainer.SetActive(false);
            }
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
            _clampToggle.Value = _instanceTrailData.ClampTexture;
        }

        private void SetLength(RangeValuesTextSlider slider, float val)
        {
            _instanceTrailData.Length = (int) val;
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

        private void SetWhitestep(RangeValuesTextSlider slider, float val)
        {
            _instanceTrailData.WhiteStep = val;
        }

        private void SetClampMode(bool shouldClamp)
        {
            _instanceTrailData.ClampTexture = shouldClamp;
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
                _instanceTrailData.RevertMaterialForCustomSaber(cs.Model as CustomSaberModel);
            }
        }

        private void CreateTrail(SaberInstance saberInstance)
        {
            _trailPreviewer.Destroy();

            _lengthSlider.RemoveEvent();
            _widthSlider.RemoveEvent();
            _whitestepSlider.RemoveEvent();
            _clampToggle.RemoveEvent();

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
                CreateTrailHand(saberInstance, trailData);
                RefreshButtonActive = true;
            }
            else
            {
                CreateTrailPedestal(saberInstance, trailData);
                RefreshButtonActive = false;
            }

            _lengthSlider.AddEvent(SetLength);
            _widthSlider.AddEvent(SetWidth);
            _whitestepSlider.AddEvent(SetWhitestep);
            _clampToggle.SetEvent(SetClampMode);
        }

        private void CreateTrailPedestal(SaberInstance saberInstance, InstanceTrailData trailData)
        {
            _trailPreviewer.Create(saberInstance.GameObject.transform.parent, trailData);

            LoadFromModel(trailData);

            _trailPreviewer.SetColor(_playerDataModel.playerData.colorSchemesSettings.GetSelectedColorScheme().saberAColor);
        }

        private void CreateTrailHand(SaberInstance saberInstance, InstanceTrailData trailData)
        {
            LoadFromModel(trailData);

            _lengthSlider.AddEvent(SetLength);
            _widthSlider.AddEvent(SetWidth);
            _whitestepSlider.AddEvent(SetWhitestep);
            _clampToggle.SetEvent(SetClampMode);
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
            _chooseTrailPopup.Show(_mainAssetStore.GetAllMetaData(AssetTypeDefinition.CustomSaber), TrailPopupSelectionChanged);
        }

        [UIAction("refresh-trail")]
        private void RefreshTrail()
        {
            _editorInstanceManager.CurrentSaber.DestroyTrail();
            _editorInstanceManager.CurrentSaber.CreateTrail();
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
