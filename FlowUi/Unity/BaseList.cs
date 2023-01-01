using System.Collections;
using HMUI;
using UnityEngine;

namespace SaberFactory.UI.Flow
{
    public abstract class BaseList : MonoBehaviour, TableView.IDataSource
    {
        [SerializeField] protected TableView _tableView;

        protected bool IsInitialized;
        
        private void Init()
        {
            if (!IsInitialized)
            {
                IsInitialized = true;
                _tableView.SetDataSource(this, true);
                InitList();
            }
        }
        
        public void SetData(object entries)
        {
            Init();
            RectTransform rectTransform = (RectTransform)_tableView.transform;
            
            rectTransform.offsetMin = Vector2.zero;
            
            DidSetData(entries);

            _tableView.ReloadData();
            _tableView.ScrollToCellWithIdx(0, TableView.ScrollPositionType.Beginning, false);

            RecalculateCells();
        }

        protected void RecalculateCells()
        {
            StartCoroutine(LateLayoutRebuild());
        }
        
        private IEnumerator LateLayoutRebuild()
        {
            yield return new WaitForEndOfFrame();
            _tableView.RefreshCellsContent();
        }
        
        public virtual float CellSize()
        {
            return 8;
        }

        public abstract int NumberOfCells();

        public abstract TableCell CellForIdx(TableView tableView, int idx);

        protected abstract void DidSetData(object cellData);
        protected abstract void InitList();
    }
}