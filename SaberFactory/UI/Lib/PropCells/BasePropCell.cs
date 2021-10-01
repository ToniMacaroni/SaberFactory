using System;
using UnityEngine;

namespace SaberFactory.UI.Lib.PropCells
{
    public abstract class BasePropCell : MonoBehaviour
    {
        public string ContentLocation => string.Join(".", GetType().Namespace, GetType().Name, "bsml");
        public Action<object> OnChangeCallback;

        public abstract void SetData(PropertyDescriptor data);
    }
}