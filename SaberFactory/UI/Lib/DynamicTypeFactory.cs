using UnityEngine;

namespace SaberFactory.UI.Lib
{
    internal class DynamicTypeFactory<TRet> : ComponentPlaceholderFactory<TRet> where TRet : Component
    {
        public T Create<T>(GameObject gameObject) where T : TRet
        {
            return (T) Create(gameObject, typeof(T));
        }
    }
}