using System;
using System.Collections.Generic;
using System.Linq;
using FlowUi.Runtime;
using HMUI;
using UnityEngine;

namespace SaberFactory.UI.Flow
{
    public partial class SaberList : BaseList
    {
        [SerializeField] private float _cellSize = 12;

        [SerializeField] private SaberCell _saberCellPrefab;
        [SerializeField] private FolderCell _folderCellPrefab;

        [SerializeField] private string _saberCellReuseId = "SaberCell";
        [SerializeField] private string _folderCellReuseId = "FolderCell";

        private List<IFolderInfo> _folderEntries;
        private List<IAssetInfo> _saberEntries;
        
        private int FolderLength => _folderEntries == null ? 0 : _folderEntries.Count;
        private int SaberLength => _saberEntries == null ? 0 : _saberEntries.Count;
        private int CombindedLength => FolderLength + SaberLength;

        public event Action<IAssetInfo> OnAssetSelected;
        public event Action<IFolderInfo> OnFolderSelected;

        public event Action<IAssetInfo> OnAssetFavoriteRequested;
        public event Action<IAssetInfo> OnAssetDeleteRequested; 

#if UNITY_EDITOR
        internal class MockFolderCell : IFolderInfo
        {
            public string Name => "Mock Folder";
        }
        
        internal class MockSaberCell : IAssetInfo
        {
            public string Name => "Mock Saber";
            public string Author => "Toni Macaroni";
            public string SubDir { get; }
            public bool IsFavorite { get; }
            public Sprite Cover { get; }
        }

        private void Start()
        {
            var folders = new List<IFolderInfo>
            {
                new MockFolderCell(),
                new MockFolderCell(),
                new MockFolderCell(),
                new MockFolderCell(),
            };

            var sabers = new List<IAssetInfo>
            {
                new MockSaberCell(),
                new MockSaberCell(),
                new MockSaberCell(),
                new MockSaberCell(),
                new MockSaberCell(),
            };
            
            SetData((folders, sabers));
        }
#endif

        protected override void InitList()
        {
            _tableView.didSelectCellWithIdxEvent += HandleDidSelectRowEvent;
        }

        private void HandleDidSelectRowEvent(TableView tableView, int idx)
        {
            if (idx < _folderEntries.Count)
            {
                OnFolderSelected?.Invoke(_folderEntries[idx]);
                return;
            }
            
            OnAssetSelected?.Invoke(_saberEntries[idx-FolderLength]);
        }

        protected override void DidSetData(object cellData)
        {
            (_folderEntries, _saberEntries) = ((List<IFolderInfo>, List<IAssetInfo>))cellData;
        }

        public bool SelectAsset(IAssetInfo asset, bool callbackTable = false, bool scroll = false)
        {
            var idx = _saberEntries.IndexOf(asset);
            if (idx > -1)
            {
                _tableView.SelectCellWithIdx(idx+FolderLength, callbackTable);
                if (scroll)
                {
                    _tableView.ScrollToCellWithIdx(idx+FolderLength, TableView.ScrollPositionType.Center, false);
                }
                return true;
            }

            return false;
        }

        public bool SelectAsset(Func<IAssetInfo, bool> filter, bool callbackTable = false, bool scroll = false)
        {
            var asset = _saberEntries.FirstOrDefault(filter);
            if (asset == null)
            {
                return false;
            }

            return SelectAsset(asset, callbackTable, scroll);
        }

        public void ClearSelection(bool callbackTable = false)
        {
            _tableView.SelectCellWithIdx(-1, callbackTable);
        }

        public override float CellSize() => _cellSize;

        public override int NumberOfCells() => CombindedLength;

        public override TableCell CellForIdx(TableView tableView, int idx)
        {
            if (idx < _folderEntries.Count)
            {
                // Its a folder entry
                var folderCell = tableView.DequeueReusableCellForIdentifier(_saberCellReuseId) as FolderCell;
                if (!folderCell)
                {
                    folderCell = Instantiate(_folderCellPrefab);
                    folderCell.reuseIdentifier = _folderCellReuseId;
                }
                folderCell.SetData(_folderEntries[idx]);
                return folderCell;
            }
            
            var saberCell = tableView.DequeueReusableCellForIdentifier(_saberCellReuseId) as SaberCell;
            if (!saberCell)
            {
                saberCell = Instantiate(_saberCellPrefab);
                saberCell.reuseIdentifier = _saberCellReuseId;
            }

            var saberIdx = idx - FolderLength;

            saberCell.SetData(_saberEntries[saberIdx]);
            
            #if !UNITY_EDITOR
            saberCell.FavoriteButton.gameObject.SetActive(OnAssetFavoriteRequested?.GetInvocationList().Length > 0);
            saberCell.DeleteButton.gameObject.SetActive(OnAssetDeleteRequested?.GetInvocationList().Length > 0);
            #endif

            saberCell.OnFavorite = () =>
            {
                OnAssetFavoriteRequested?.Invoke(_saberEntries[saberIdx]);
            };
            saberCell.OnDelete = () =>
            {
                OnAssetDeleteRequested?.Invoke(_saberEntries[saberIdx]);
            };
            
            return saberCell;
        }
    }
}