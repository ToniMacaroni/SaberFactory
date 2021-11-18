using SaberFactory.UI.Lib;

namespace SaberFactory.UI
{
    internal class SaberFactoryMenuButton : MenuButtonRegistrar
    {
        private readonly Editor.Editor _editor;

        protected SaberFactoryMenuButton(Editor.Editor editor) : base("Saber Factory", "Good quality sabers")
        {
            _editor = editor;
        }

        protected override void OnClick()
        {
            _editor.Open();
        }
    }
}