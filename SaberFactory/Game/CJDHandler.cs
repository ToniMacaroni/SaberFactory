using System;
using System.Reflection;

namespace SaberFactory.Game
{
    internal static class CJDHandler
    {
        private static bool _gotCJDTypes;
        private static MethodInfo _cjdAtMethod;
        private static PropertyInfo _cjdLevelCustomDataType;

        public static bool GetField(this BeatmapData beatmap, string field, out object obj)
        {
            try
            {
                if (!_gotCJDTypes)
                {
                    GetTypes();
                }

                if (_cjdAtMethod == null || _cjdLevelCustomDataType == null)
                {
                    obj = null;
                    return false;
                }

                obj = _cjdAtMethod?.Invoke(null, new[] { _cjdLevelCustomDataType?.GetValue(beatmap), field });
                if (obj == null) return false;
                return true;
            }
            catch (Exception)
            {
                obj = null;
                return false;
            }
        }

        private static void GetTypes()
        {
            _gotCJDTypes = true;

            var pluginMetadata = IPA.Loader.PluginManager.GetPluginFromId("CustomJSONData");
            if (pluginMetadata == null) return;

            var customBeatmapDataType = pluginMetadata.Assembly.GetType("CustomJSONData.CustomBeatmap.CustomBeatmapData");

            var treesType = pluginMetadata.Assembly.GetType("CustomJSONData.Trees");

            _cjdAtMethod = treesType?.GetMethod("at", BindingFlags.Static | BindingFlags.Public);
            _cjdLevelCustomDataType = customBeatmapDataType?.GetProperty("levelCustomData", BindingFlags.Public | BindingFlags.Instance);
        }
    }
}