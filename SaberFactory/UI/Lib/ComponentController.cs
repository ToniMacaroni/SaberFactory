using BeatSaberMarkupLanguage.Components;

namespace SaberFactory.UI.Lib
{
    internal abstract class ComponentController
    {
        public ExternalComponents ExternalComponents;
        
        public abstract void RemoveEvent();

        public abstract string GetId();

        public abstract void SetValue(object val);

        public abstract object GetValue();
    }
}