using System;

namespace SaberFactory.Installers
{
    internal class SaberSettableSettings
    {
        public SettableSettingAbstraction<bool> RelativeTrailMode = new SettableSettingAbstraction<bool>();

        public class SettableSettingAbstraction<T>
        {
            public event Action ValueChanged;

            public T Value
            {
                get => _value;
                set
                {
                    _value = value;
                    ValueChanged?.Invoke();
                }
            }

            private T _value;

        }
    }
}