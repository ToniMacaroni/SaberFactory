using System;
using System.Collections.Generic;
using System.Linq;
using HMUI;
using UnityEngine;

namespace SaberFactory.UI.Flow
{
    public partial class PresetList : BaseList
    {
        [SerializeField] private float _cellSize = 12;

        [SerializeField] private PresetCell _presetCellPrefab;

        [SerializeField] private string _presetCellReuseId = "PresetCell";

        private List<IPresetInfo> _presetEntries;

        public event Action<IPresetInfo> OnPresetSelected;
        public event Action<IPresetInfo> OnPresetDeleted;
        public event Action<IPresetInfo, bool> OnPresetMonitorToggled;

        protected override void InitList()
        {
            _tableView.didSelectCellWithIdxEvent += HandleDidSelectRowEvent;
        }

        private void HandleDidSelectRowEvent(TableView tableView, int idx)
        {
            OnPresetSelected?.Invoke(_presetEntries[idx]);
        }

        protected override void DidSetData(object cellData)
        {
            _presetEntries = (List<IPresetInfo>)cellData;
        }

        public void SelectAsset(IPresetInfo asset, bool callbackTable = false)
        {
            var idx = _presetEntries.IndexOf(asset);
            if (idx > -1)
            {
                _tableView.SelectCellWithIdx(idx, callbackTable);
            }
        }

        public void SelectAsset(Func<IPresetInfo, bool> filter)
        {
            var asset = _presetEntries.FirstOrDefault(filter);
            if (asset == null)
            {
                return;
            }
            
            SelectAsset(asset);
        }

        public void ClearSelection()
        {
            _tableView.SelectCellWithIdx(-1);
        }

        public override float CellSize() => _cellSize;

        public override int NumberOfCells() => _presetEntries == null ? 0 : _presetEntries.Count;

        public override TableCell CellForIdx(TableView tableView, int idx)
        {
            var presetCell = tableView.DequeueReusableCellForIdentifier(_presetCellReuseId) as PresetCell;
            if (!presetCell)
            {
                presetCell = Instantiate(_presetCellPrefab);
                presetCell.reuseIdentifier = _presetCellReuseId;
            }
            
            var preset = _presetEntries[idx];

            presetCell.SetData(preset);
            
            presetCell.OnDeletePressed = () =>
            {
                OnPresetDeleted?.Invoke(preset);
            };
            
            presetCell.OnMonitorModeChanged = isOn =>
            {
                OnPresetMonitorToggled?.Invoke(preset, isOn);
            };

            return presetCell;
        }
    }
}