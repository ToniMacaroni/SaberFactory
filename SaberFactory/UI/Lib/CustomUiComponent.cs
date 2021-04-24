using Zenject;

namespace SaberFactory.UI.Lib
{
    internal class CustomUiComponent : CustomParsable
    {
        internal class Factory : ComponentPlaceholderFactory<CustomUiComponent>
        {
        }
    }
}