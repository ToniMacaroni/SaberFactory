using System.Threading.Tasks;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components.Settings;
using HMUI;
using SaberFactory.Editor;
using SaberFactory.Instances;
using SaberFactory.Instances.CustomSaber;
using SaberFactory.Instances.Trail;
using SaberFactory.Models;
using SaberFactory.UI.Lib;
using UnityEngine;
using Zenject;


namespace SaberFactory.UI.CustomSaber.Views
{
    internal class TrailSettingsView : SubView
    {
        [Inject] private readonly TrailPreviewer _trailPreviewer = null;
        [Inject] private readonly EditorInstanceManager _editorInstanceManager = null;
        [Inject] private readonly ColorManager _colorManager = null;

        private InstanceTrailData _instanceTrailData;

        [UIComponent("length-slider")] private readonly SliderSetting _lengthSliderSetting = null;
        [UIComponent("width-slider")] private readonly SliderSetting _widthSliderSetting = null;
        [UIComponent("whitestep-slider")] private readonly SliderSetting _whitestepSliderSetting = null;

        private SliderController _lengthSlider;
        private SliderController _widthSlider;
        private SliderController _whitestepSlider;

        [UIAction("#post-parse")]
        private void Setup()
        {
            _lengthSlider = new SliderController(_lengthSliderSetting);
            _widthSlider = new SliderController(_widthSliderSetting);
            _whitestepSlider = new SliderController(_whitestepSliderSetting);
        }

        public override void DidOpen()
        {
            CreateTrail(_editorInstanceManager.CurrentSaber);

            _editorInstanceManager.OnSaberInstanceCreated += CreateTrail;
        }

        public override void DidClose()
        {
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
        }

        private void SetLength(RangeValuesTextSlider slider, float val)
        {
            _instanceTrailData.SetLength((int)val);
            _trailPreviewer.SetLength(val);
        }

        private void SetWidth(RangeValuesTextSlider slider, float val)
        {
            _instanceTrailData.SetWidth(val);
            _trailPreviewer.UpdateWidth();
        }

        private void SetWhitestep(RangeValuesTextSlider slider, float val)
        {
            _instanceTrailData.SetWhitestep(val);
        }

        private void ResetTrail()
        {
            if (_editorInstanceManager.CurrentPiece is CustomSaberInstance customsaber)
            {
                customsaber.ResetTrail();
            }
        }

        private void CreateTrail(SaberInstance saberInstance)
        {
            _lengthSlider.RemoveEvent(SetLength);
            _widthSlider.RemoveEvent(SetWidth);
            _whitestepSlider.RemoveEvent(SetWhitestep);

            var trailData = saberInstance?.GetTrailData();
            if (trailData == null) return;

            _trailPreviewer.Create(saberInstance.GameObject.transform.parent, trailData);

            LoadFromModel(trailData);

            _lengthSlider.AddEvent(SetLength);
            _widthSlider.AddEvent(SetWidth);
            _whitestepSlider.AddEvent(SetWhitestep);

            _trailPreviewer.SetColor(_colorManager.ColorForSaberType(SaberType.SaberA));
        }

        private void UpdateProps()
        {
            ParserParams.EmitEvent("update-props");
        }
    }
}
