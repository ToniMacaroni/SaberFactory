using SaberFactory.UI.Lib;

namespace SaberFactory.UI
{
    internal class SaberFactoryMenuButton : MenuButtonRegistrar
    {
        private readonly Editor.LegacyEditor _editor;

        protected SaberFactoryMenuButton(Editor.LegacyEditor editor) : base("Saber Factory", "Good quality sabers")
        {
            _editor = editor;
        }

        protected override void OnClick()
        {
            _editor.Open();
        }
    }
}