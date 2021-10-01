using System;

namespace SaberFactory.UI.Lib
{
    public class PropertyDescriptor
    {
        public object AddtionalData;
        public Action<object> ChangedCallback;
        public object PropObject;
        public string Text;
        public EPropertyType Type;

        public PropertyDescriptor(string text, EPropertyType type, object propObject, Action<object> changedCallback)
        {
            Text = text;
            Type = type;
            PropObject = propObject;
            ChangedCallback = changedCallback;
        }
    }
}