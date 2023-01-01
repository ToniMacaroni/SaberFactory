using System;
using System.Threading.Tasks;
using HMUI;
using UnityEngine;
using Zenject;

namespace FlowUi.Runtime
{
    public class CopyInHere : MonoBehaviour
    {
        [Inject] private DiContainer _container;
        public Transform parent;
        
        public void Start()
        {
            Init();
        }

        public async void Init()
        {
            await Task.Delay(3000);
            Debug.Log("Loading");
            foreach (Transform t in parent)
            {
                DestroyImmediate(t.gameObject);
            }
            
            var bundle = AssetBundle.LoadFromFile(@"F:\BeatSaberUnity\AssetBundles\StandaloneWindows\SaberFactoryMainMenuView");
            
            var ui = Instantiate(bundle.LoadAsset<GameObject>("ui_prefab"));
            ui.transform.SetParent(parent, false);
            
            _container.InjectGameObject(ui);
            
            //var root = ui.GetComponent<UIRoot>();
            //root.ClickSignal.Subscribe(evnt);
            
            ui.SetActive(true);
            Debug.Log("Loaded");
        }
    }
}