using SaberFactory.DataStore;
using SaberFactory.Editor;
using UnityEngine;
using Zenject;

namespace SaberFactory.Helpers
{
    public class DebugMenu : MonoBehaviour
    {
        [Inject] private readonly DiContainer _container = null;
        
        private void Start()
        {
            Debug.LogWarning("Hello");
        }

        private void OnGUI()
        {
            GUI.matrix = Matrix4x4.TRS(new Vector3(2, 2, 1), Quaternion.identity, new Vector3(2, 2, 1));
            
            GUILayout.BeginVertical();
            if (GUILayout.Button("Run Debug Action"))
            {
                RunDebugAction();
            }
            GUILayout.EndVertical();
        }

        private async void RunDebugAction()
        {
            var editorInstanceManager = _container.Resolve<EditorInstanceManager>();
            var assetStore = _container.Resolve<MainAssetStore>();
            var saber = await assetStore["CustomSabers\\Mod Test.saber"];
            editorInstanceManager.SetModelComposition(saber);
            var editor = _container.Resolve<Editor.Editor>();
            editor.Open();
        }
    }
}