using System.Diagnostics.CodeAnalysis;
using HMUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FlowUi.Runtime
{
    public class FlowTableCell : TableCell
    {
        [SerializeField]
        private CanvasGroup _canvasGroup;
        
        [SerializeField]
        private Image _backgroundImage;

        [Space]
        [SerializeField]
        private Color _highlightBackgroundColor;

        [SerializeField]
        private Color _selectedBackgroundColor;

        [SerializeField]
        private Color _selectedAndHighlightedBackgroundColor;
        
        public virtual void SetData(object data)
        {
        }

        protected override void SelectionDidChange(TransitionType transitionType)
        {
            RefreshVisuals();
        }

        protected override void HighlightDidChange(TransitionType transitionType)
        {
            RefreshVisuals();
        }

        private void RefreshVisuals()
        {
            if (base.selected && base.highlighted)
            {
                _backgroundImage.color = _selectedAndHighlightedBackgroundColor;
            }
            else if (base.highlighted)
            {
                _backgroundImage.color = _highlightBackgroundColor;
            }
            else if (base.selected)
            {
                _backgroundImage.color = _selectedBackgroundColor;
            }
            _backgroundImage.enabled = base.selected || base.highlighted;
        }
    }
}