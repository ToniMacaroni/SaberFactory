using UnityEngine;

namespace FlowUi
{
    public static class InstanceDictionary
    {
        public static readonly LazyReference<Signal> ClickSignal = new("UIButtonWasPressed");
    }
}