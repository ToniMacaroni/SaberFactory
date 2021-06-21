using SaberFactory.Editor;
using SaberFactory.Helpers;
using SaberFactory.Instances;
using SaberFactory.Instances.CustomSaber;
using SaberFactory.Instances.Setters;
using SaberFactory.UI.Lib;
using Zenject;


namespace SaberFactory.UI.CustomSaber.Views
{
    internal class TransformSettingsView : SubView, INavigationCategoryView
    {
        public ENavigationCategory Category => ENavigationCategory.Transform;

        public float RotationAmount
        {
            get
            {
                if (_transformDataSetter == null) return 0f;
                return _transformDataSetter.Rotation;
            }
            set
            {
                if (_transformDataSetter == null) return;
                _transformDataSetter.Rotation = value;
            }
        }

        public float OffsetAmount
        {
            get
            {
                if (_transformDataSetter == null) return 0f;
                return _transformDataSetter.Offset;
            }
            set
            {
                if (_transformDataSetter == null) return;
                _transformDataSetter.Offset = value;
            }
        }

        [Inject] private readonly EditorInstanceManager _editorInstanceManager = null;

        private TransformDataSetter _transformDataSetter;

        public override void DidOpen()
        {
            if (_editorInstanceManager.CurrentPiece is CustomSaberInstance cs)
            {
                _transformDataSetter = cs.PropertyBlockSetterHandler.Cast<CustomSaberPropertyBlockSetterHandler>()
                    .TransformDataSetter;
            }

            ParserParams.EmitEvent("update-props");

        }

        public override void DidClose()
        {
            _transformDataSetter = null;
        }
    }
}
