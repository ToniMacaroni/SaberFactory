using System;
using System.Collections.Generic;
using System.Linq;
using HMUI;
using ModestTree;
using Tweening;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif
using UnityEngine;
using Zenject;

namespace FlowUi.Runtime
{
    public class TabController : CustomSerializable
    {
        [SerializeField] private List<TabData> _data;

        [SerializeField] private IconSegmentedControl _segmentedControl;

        [SerializeField]
        private RectTransform _selectionIndicator;

        [Inject]
        private readonly TimeTweeningManager _tweeningManager = null;

        public event Action<TabData> OnTabSelected;

        public override void Save()
        {
            Serialize(_data);
        }

        // [Button(ButtonSizes.Medium)]
        // private void PrintValue()
        // {
        // }

        public override void Construct()
        {
            Deserialize(ref _data);
            Init();
        }
        
        #if UNITY_EDITOR
        [Button]
        private void FillTable()
        {
            var container = new DiContainer();
            container.BindInstance(GameObject.Find("HoverHintController").GetComponent<HoverHintController>()).AsSingle();
            container.Inject(_segmentedControl);
            _segmentedControl.dataSource = _segmentedControl;
            _segmentedControl.SetData(_data.Select(x=>x.ToDataItem()).ToArray());
        }

        [Button]
        private void ClearTable()
        {
            _segmentedControl.SetData(Array.Empty<IconSegmentedControl.DataItem>());
        }
        #endif

        public void Init()
        {
            _segmentedControl.SetData(_data.Select(x => x.ToDataItem()).ToArray());
            _segmentedControl.didSelectCellEvent += SegmentedControlOndidSelectCellEvent;

            // _selectionIndicator.position = _segmentedControl.cells[0].transform.position;
        }

        private void OnDestroy()
        {
            _segmentedControl.didSelectCellEvent -= SegmentedControlOndidSelectCellEvent;
        }

        private void AnimateIndicator(int cellIdx)
        {
            var cell = _segmentedControl.cells[cellIdx];
            
            _tweeningManager.KillAllTweens(this);
            _tweeningManager.AddTween(new Vector3Tween(_selectionIndicator.position, cell.transform.position, val =>
                {
                    _selectionIndicator.position = val;
                }, 0.28f, EaseType.InOutCubic),
                this);
        }

        private void SegmentedControlOndidSelectCellEvent(SegmentedControl control, int idx)
        {
            AnimateIndicator(idx);
            
            var tab = _data[idx];
            OnTabSelected?.Invoke(tab);
            tab.Select();
        }
    }
}