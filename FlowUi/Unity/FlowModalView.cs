using HMUI;
using UnityEngine;
using UnityEngine.UI;

namespace FlowUi.Runtime
{
    public class FlowModalView : CustomModalView
    {
        public virtual void Show()
        {
            Show(true);
            
            var vc = transform.GetComponentInParent<FlowViewController>();
            if (vc)
            {
                vc.OnFlowDidDeactivate += HandleFlowViewcontrollerDidDeactivate;
            }
            
            _blockerGO.GetComponent<Button>().onClick.AddListener(Hide);
        }
        
        public void HandleFlowViewcontrollerDidDeactivate()
        {
            Hide(false);
        }

        public virtual void Hide()
        {
            Hide(true);
        }

        public void Hide(bool animated)
        {
            if (!_isShown)
            {
                return;
            }

            var blockerButton = _blockerGO.GetComponent<Button>();
            blockerButton.onClick.RemoveListener(Hide);
            base.Hide(animated);
        }

        public override void OnDisable()
        {
             var vc = transform.GetComponentInParent<FlowViewController>();
             if (vc)
             {
                 vc.OnFlowDidDeactivate -= HandleFlowViewcontrollerDidDeactivate;
             }
             
             base.OnDisable();
        }
    }
}