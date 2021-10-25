using UnityEditor;

namespace SaberFactory.Editor
{
    internal class ObjectCreator
    {
        [MenuItem("Assets/Create/Saber Factory/Saber Options")]
        public static void CreateSaberOptions()
        {
            // var asset = ScriptableObject.CreateInstance<SaberFactoryOptions>();
            //
            // AssetDatabase.CreateAsset(asset, "Assets/SaberOptions.asset");
            // AssetDatabase.SaveAssets();
            //
            // EditorUtility.FocusProjectWindow();
            //
            // Selection.activeObject = asset;
        }
    }
}