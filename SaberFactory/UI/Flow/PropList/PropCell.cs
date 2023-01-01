using System;
using UnityEngine;

namespace SaberFactory.UI.Flow
{
    public partial class PropCell : MonoBehaviour
    {
        [SerializeField] private float _cellHeight = 14;

        private void Awake()
        {
            var rect = GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, _cellHeight);
        }

        public virtual void ResetClicked()
        {
            
        }
    }
}