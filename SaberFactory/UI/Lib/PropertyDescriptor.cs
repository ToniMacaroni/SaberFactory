using System;

namespace SaberFactory.UI.Lib
{
    public class PropertyDescriptor
    {
        public string Text;
        public EPropertyType Type;
        public object PropObject;
        public Action<object> ChangedCallback;

        public object AddtionalData;

        public PropertyDescriptor(string text, EPropertyType type, object propObject, Action<object> changedCallback)
        {
            Text = text;
            Type = type;
            PropObject = propObject;
            ChangedCallback = changedCallback;
        }
    };
}