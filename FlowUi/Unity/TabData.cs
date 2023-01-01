using System;
using HMUI;
using UnityEngine;

namespace FlowUi.Runtime
{
    [Serializable]
    public class TabData
    {
        public string Key;
        public string HintText;
        public Sprite Icon;
        public object Data;
        public event Action OnSelected;
        private IconSegmentedControl.DataItem _dataItem;

        public void Select()
        {
            OnSelected?.Invoke();
        }

        public IconSegmentedControl.DataItem ToDataItem()
        {
            if (_dataItem == null)
            {
                _dataItem = new IconSegmentedControl.DataItem(Icon, HintText);
            }

            return _dataItem;
        }
    }
}