using System.Threading.Tasks;
using UnityEngine;

namespace FlowUi.Runtime
{
    public class FlowCategoryContainer : MonoBehaviour, IPreExport
    {
        public virtual int ContainerId => -1;

        protected bool _init;
        protected bool _isOpen;
        protected FlowViewController _parentViewController;

        protected virtual Task DidOpen()
        {
            return Task.CompletedTask;
        }

        protected virtual Task DidClose()
        {
            return Task.CompletedTask;
        }

        protected virtual Task Setup()
        {
            return Task.CompletedTask;
        }

        public async Task Open(FlowViewController parentViewController)
        {
            if (_isOpen)
            {
                return;
            }

            _isOpen = true;
            
            _parentViewController = parentViewController;
            
            if (!_init)
            {
                await Setup();
                _init = true;
            }
            
            gameObject.SetActive(true);

            await DidOpen();
        }

        public async Task Close()
        {
            if (!_isOpen)
            {
                return;
            }

            _isOpen = false;
            
            await DidClose();
            gameObject.SetActive(false);
        }

        public void PreExport()
        {
            gameObject.SetActive(false);
        }
    }
}