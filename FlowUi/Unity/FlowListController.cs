using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HMUI;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Object = UnityEngine.Object;

namespace FlowUi.Runtime
{
    public class FlowListController : MonoBehaviour, TableView.IDataSource
    {
        [SerializeField] private TableView _tableView;

        [SerializeField] private AlphabetScrollbar _alphabetScrollbar;

        [SerializeField] private FlowTableCell _cellPrefab;
        [SerializeField] private FlowTableCell _headerCellPrefab;

        [SerializeField] private string _levelCellsReuseIdentifier = "LevelCell";

        [SerializeField] private string _packCellsReuseIdentifier = "PackCell";

        [SerializeField] private float _cellHeight = 8.5f;

        [SerializeField] private int _showAlphabetScrollbarLevelCountThreshold = 16;

        private bool _isInitialized;
        private int _selectedRow = -1;

        public class ListEntry
        {
            public string Text;
            public Sprite Sprite;
            public object LinkedObject;
        }

        private List<ListEntry> _entries;

        private ListEntry _selectedEntry;

        public event Action<FlowListController, ListEntry> didSelectHeader;
        public event Action<FlowListController, ListEntry> didSelectEntry;

#if UNITY_EDITOR
        private void Start()
        {
            FillList();
        }
#endif

        private void Init()
        {
            if (!_isInitialized)
            {
                _isInitialized = true;
                _tableView.SetDataSource(this, true);
                _tableView.didSelectCellWithIdxEvent += HandleDidSelectRowEvent;
            }
        }
        
        #if UNITY_EDITOR
        [Button(ButtonSizes.Medium)]
        private void FillList()
        {
            _tableView.SetDataSource(this, false);
            var entries = new List<ListEntry>();
            for (int i = 0; i < 50; i++)
            {
                entries.Add(new ListEntry {Text = i.ToString(), Sprite = null});
            }
            SetData(entries);
        }

        [Button]
        private void ClearList()
        {
            SetData(new List<ListEntry>());
            foreach (Transform t in _tableView.contentTransform)
            {
                DestroyImmediate(t.gameObject);
            }
        }
        #endif

        public void SetData(List<ListEntry> entries)
        {
            Init();
            _entries = entries;
            RectTransform rectTransform = (RectTransform)_tableView.transform;
            
            rectTransform.offsetMin = new Vector2(0f, 0f);
            _alphabetScrollbar.gameObject.SetActive(false);

            _tableView.ReloadData();
            _tableView.ScrollToCellWithIdx(0, TableView.ScrollPositionType.Beginning, false);

            StartCoroutine(LateLayoutRebuild());
        }

        private IEnumerator LateLayoutRebuild()
        {
            yield return new WaitForEndOfFrame();
            _tableView.RefreshCellsContent();
        }

        protected void OnDestroy()
        {
            if (_tableView != null)
            {
                _tableView.didSelectCellWithIdxEvent -= HandleDidSelectRowEvent;
            }
        }

        public float CellSize()
        {
            return _cellHeight;
        }

        public int NumberOfCells()
        {
            if (_entries == null)
            {
                return 0;
            }

            return _entries.Count;
        }

        public TableCell CellForIdx(TableView tableView, int row)
        {
            // if (row == 0 && _showLevelPackHeader)
            // {
            //     var levelPackHeaderTableCell =
            //         tableView.DequeueReusableCellForIdentifier(_packCellsReuseIdentifier);
            //     if (!levelPackHeaderTableCell)
            //     {
            //         levelPackHeaderTableCell = Instantiate(_headerCellPrefab);
            //         levelPackHeaderTableCell.reuseIdentifier = _packCellsReuseIdentifier;
            //     }
            //
            //     //levelPackHeaderTableCell.SetData(_headerText);
            //     return levelPackHeaderTableCell;
            // }

            var cell = tableView.DequeueReusableCellForIdentifier(_levelCellsReuseIdentifier) as FlowTableCell;
            if (!cell)
            {
                cell = Instantiate(_cellPrefab);
                cell.reuseIdentifier = _levelCellsReuseIdentifier;
            }

            var entry = _entries[row];
            
            cell.SetData(entry);
            return cell;
        }

        private void HandleDidSelectRowEvent(TableView tableView, int row)
        {
            _selectedRow = row;
            _selectedEntry = _entries[row];
            didSelectEntry?.Invoke(this, _selectedEntry);
        }

        private void SilentReload()
        {
            _tableView.ReloadDataKeepingPosition();
            _selectedRow = Math.Min(_selectedRow, NumberOfCells() - 1);
            _tableView.SelectCellWithIdx(_selectedRow);
        }

        public void SelectHeader()
        {
            _selectedRow = 0;
            _tableView.SelectCellWithIdx(0);
        }

        public void ClearSelection()
        {
            _selectedRow = -1;
            _tableView.SelectCellWithIdx(_selectedRow);
        }

        public void SelectEntry(ListEntry entry, bool callback = true)
        {
            if (entry == null)
            {
                return;
            }
            
            var idx = _entries.IndexOf(entry);

            if (idx >= 0)
            {
                _selectedRow = idx;
                _tableView.SelectCellWithIdx(idx, true);
                _tableView.ScrollToCellWithIdx(idx, TableView.ScrollPositionType.Center, false);
            }
        }

        public void SelectEntry(Func<ListEntry, bool> filter, bool callback = true)
        {
            SelectEntry(_entries.FirstOrDefault(filter), callback);
        }
    }
}