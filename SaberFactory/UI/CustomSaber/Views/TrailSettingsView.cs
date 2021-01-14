using System.Threading.Tasks;
using BeatSaberMarkupLanguage.Attributes;
using SaberFactory.Editor;
using SaberFactory.Instances;
using SaberFactory.Models;
using SaberFactory.UI.Lib;
using Zenject;


namespace SaberFactory.UI.CustomSaber.Views
{
    internal class TrailSettingsView : SubView
    {
        [Inject] private readonly TrailPreviewer _trailPreviewer = null;
        [Inject] private readonly EditorInstanceManager _editorInstanceManager = null;

        private int _length;
        private float _width;
        private TrailModel _trailModel;


        [UIValue("value-length")]
        private int Length
        {
            get => _length;
            set => SetLength(value);
        }

        [UIValue("value-width")]
        private float Width
        {
            get => _width;
            set => SetWidth(value);
        }

        public override void DidOpen()
        {
            CreateTrail(_editorInstanceManager.CurrentSaber);

            if (_editorInstanceManager.CurrentSaber != null)
            {
                LoadFromModel(_editorInstanceManager.CurrentSaber);
            }

            _editorInstanceManager.OnSaberInstanceCreated += CreateTrail;
        }

        public override void DidClose()
        {
            _trailModel.TrailLengthOffset = _length;
            _trailModel.TrailWidthOffset = _width;

            _trailPreviewer.Destroy();
            _editorInstanceManager.OnSaberInstanceCreated -= CreateTrail;
        }

        private void LoadFromModel(SaberInstance saberInstance)
        {
            if (saberInstance == null) return;
            var trailData = saberInstance.GetTrailData();
            var saberModel = saberInstance.Model;
            var firstInit = saberModel.InitTrailModel();

            var modelNullable = saberModel.GetTrailModel();
            if (!modelNullable.HasValue) return;
            _trailModel = modelNullable.Value;

            if (firstInit)
            {
                _trailModel.Material = trailData.Material;
            }

            _length = _trailModel.TrailLengthOffset;
            _width = _trailModel.TrailWidthOffset;

            UpdateProps();
        }

        private void SetLength(int val)
        {
            _trailPreviewer.Length = val;
        }

        private void SetWidth(float val)
        {
            _trailPreviewer.Width = val;
        }

        private void CreateTrail(SaberInstance saberInstance)
        {
            if (saberInstance == null) return;
            _trailPreviewer.Create(saberInstance.GameObject.transform, saberInstance.GetTrailData());
        }

        private void UpdateProps()
        {
            ParserParams.EmitEvent("update-props");
        }
    }
}
