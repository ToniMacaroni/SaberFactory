using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FlowUi.Runtime
{
    public class UIRoot : MonoBehaviour
    {
        public Signal ClickSignal;
        public List<FlowViewController> ViewControllers;

        public event Action didRequestExit;

        public T GetViewController<T>(string id) where T : FlowViewController
        {
            return (T)ViewControllers.FirstOrDefault(x => x.ViewControllerId == id);
        }

        public void RequestExit()
        {
            didRequestExit?.Invoke();
        }
    }
}