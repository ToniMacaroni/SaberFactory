using System.Linq;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components.Settings;
using HMUI;
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
using Zenject;


namespace SaberFactory.UI.CustomSaber.Views
{
    internal class TrailSettingsView : SubView, INavigationCategoryView
    {
        public ENavigationCategory Category => ENavigationCategory.Trail;

        [Inject] private readonly TrailPreviewer _trailPreviewer = null;
        [Inject] private readonly EditorInstanceManager _editorInstanceManager = null;
        [Inject] private readonly ColorManager _colorManager = null;
        [Inject] private readonly MainAssetStore _mainAssetStore = null;

        private InstanceTrailData _instanceTrailData;

        [UIComponent("length-slider")] private readonly SliderSetting _lengthSliderSetting = null;
        [UIComponent("width-slider")] private readonly SliderSetting _widthSliderSetting = null;
        [UIComponent("whitestep-slider")] private readonly SliderSetting _whitestepSliderSetting = null;
        [UIComponent("clamp-checkbox")] private readonly ToggleSetting _clampToggleSetting = null;

        [UIComponent("material-editor")] private readonly MaterialEditor _materialEditor = null;
        [UIComponent("choose-trail-popup")] private readonly ChooseTrailPopup _chooseTrailPopup = null;

        private SliderController _lengthSlider;
        private SliderController _widthSlider;
        private SliderController _whitestepSlider;
        private ToggleController _clampToggle;


        [UIAction("#post-parse")]
        private void Setup()
        {
            _lengthSlider = new SliderController(_lengthSliderSetting);
            _widthSlider = new SliderController(_widthSliderSetting);
            _whitestepSlider = new SliderController(_whitestepSliderSetting);
            _clampToggle = new ToggleController(_clampToggleSetting);
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
            _trailPreviewer.SetLength(val);
        }

        private void SetWidth(RangeValuesTextSlider slider, float val)
        {
            _instanceTrailData.Width = val;
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

        private void ResetTrail()
        {
            if (_editorInstanceManager.CurrentPiece is CustomSaberInstance cs)
            {
                _instanceTrailData.RevertMaterialForCustomSaber(cs.Model as CustomSaberModel);
                SetTrailModel(null);
            }
        }

        private void CreateTrail(SaberInstance saberInstance)
        {
            _lengthSlider.RemoveEvent(SetLength);
            _widthSlider.RemoveEvent(SetWidth);
            _whitestepSlider.RemoveEvent(SetWhitestep);
            _clampToggle.RemoveEvent();

            var trailData = saberInstance?.GetTrailData();
            if (trailData == null) return;

            _trailPreviewer.Create(saberInstance.GameObject.transform.parent, trailData);

            LoadFromModel(trailData);

            _lengthSlider.AddEvent(SetLength);
            _widthSlider.AddEvent(SetWidth);
            _whitestepSlider.AddEvent(SetWhitestep);
            _clampToggle.SetEvent(SetClampMode);

            _trailPreviewer.SetColor(_colorManager.ColorForSaberType(SaberType.SaberA));
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
            _trailPreviewer.Destroy();
            ResetTrail();
            _editorInstanceManager.Refresh();
        }

        [UIAction("choose-trail")]
        private void ClickChooseTrail()
        {
            _chooseTrailPopup.Show(_mainAssetStore.GetAllMetaData(AssetTypeDefinition.CustomSaber), model =>
            {
                if (model == null) return;
                _trailPreviewer.Destroy();
                SetTrailModel(model);
                _editorInstanceManager.Refresh();
            });
        }
    }
}
