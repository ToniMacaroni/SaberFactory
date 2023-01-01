using FlowUi.Helpers;
using Object = UnityEngine.Object;


namespace FlowUi
{
    public class LazyInstance<T> where T : Object
    {
        private int _instanceId;
        private T _value;

        public T Value
        {
            get
            {
                if (!_value)
                {
                    _value = GetObject();
                }

                return _value;
            }
        }

        public LazyInstance(int instanceId)
        {
            _instanceId = instanceId;
        }

        private T GetObject()
        {
            if (!UnityHelpers.TryGetObjectById(_instanceId, out T val))
            {
                // TODO: Get and save new id to disk
                throw new Exception($"Couldn't get {typeof(T).Name} with id {_instanceId}");
            }

            return val;
        }
    }
}