using System.Collections.Generic;
using FactoryCore.OdinSerializer;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FlowUi.Runtime
{
    public abstract class CustomSerializable : MonoBehaviour , ICustomSerializable
    {
        [SerializeField] [HideInInspector] protected List<Object> SerializedObjects;
        [SerializeField] [HideInInspector] protected byte[] SerializedData;

        private void Awake()
        {
            if (SerializedData != null)
            {
                Construct();
            }
        }

        public void Serialize(object obj)
        {
            SerializedData = SerializationUtility.SerializeValue(obj, DataFormat.Binary, out SerializedObjects);
        }

        public void Deserialize<T>(ref T obj)
        {
            obj = SerializationUtility.DeserializeValue<T>(SerializedData, DataFormat.Binary, SerializedObjects);
        }

        public virtual void Construct()
        {
        
        }

        public virtual void Save()
        {
        
        }
    }
}