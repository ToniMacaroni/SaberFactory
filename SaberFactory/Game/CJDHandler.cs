using System;
using System.Collections.Generic;
using System.Reflection;

namespace SaberFactory.Game
{
    internal static class CJDHandler
    {
        private static bool _initialized;
        private static ICJDHandler _cjdHandler;

        public static bool GetField(this BeatmapData beatmap, string field, out object obj)
        {
            if (!_initialized)
            {
                Init();
            }

            if (_cjdHandler == null)
            {
                obj = null;
                return false;
            }

            return _cjdHandler.GetField(beatmap, field, out obj);
        }

        private static void Init()
        {
            _initialized = true;

            var pluginMetadata = IPA.Loader.PluginManager.GetPluginFromId("CustomJSONData");
            if (pluginMetadata == null)
            {
                return;
            }

            if (pluginMetadata.HVersion.Major > 1)
            {
                _cjdHandler = new CJD2Handler();
            }
            else
            {
                _cjdHandler = new CJD1Handler();
            }

            _cjdHandler.Setup(pluginMetadata.Assembly);
        }

        internal interface ICJDHandler
        {
            public bool GetField(BeatmapData beatmap, string field, out object obj);

            public void Setup(Assembly assembly);
        }

        internal class CJD1Handler : ICJDHandler
        {
            private MethodInfo _cjdAtMethod;
            private PropertyInfo _cjdLevelCustomDataType;
            private bool _isValid;

            bool ICJDHandler.GetField(BeatmapData beatmap, string field, out object obj)
            {
                try
                {
                    if (!_isValid)
                    {
                        obj = null;
                        return false;
                    }

                    obj = _cjdAtMethod?.Invoke(null, new[] { _cjdLevelCustomDataType?.GetValue(beatmap), field });
                    if (obj == null)
                    {
                        return false;
                    }

                    return true;
                }
                catch (Exception)
                {
                    obj = null;
                    return false;
                }
            }

            public void Setup(Assembly assembly)
            {
                var customBeatmapDataType = assembly.GetType("CustomJSONData.CustomBeatmap.CustomBeatmapData");

                var treesType = assembly.GetType("CustomJSONData.Trees");

                _cjdAtMethod = treesType?.GetMethod("at", BindingFlags.Static | BindingFlags.Public);
                _cjdLevelCustomDataType = customBeatmapDataType?.GetProperty("levelCustomData", BindingFlags.Public | BindingFlags.Instance);

                _isValid = _cjdAtMethod != null && _cjdLevelCustomDataType != null;
            }
        }

        internal class CJD2Handler : ICJDHandler
        {
            private PropertyInfo _cjdLevelCustomDataType;
            private bool _isValid;

            bool ICJDHandler.GetField(BeatmapData beatmap, string field, out object obj)
            {
                try
                {
                    if (!_isValid)
                    {
                        obj = null;
                        return false;
                    }

                    var dict = _cjdLevelCustomDataType.GetValue(beatmap) as Dictionary<string, object>;

                    if (dict == null || !dict.TryGetValue(field, out obj))
                    {
                        obj = null;
                        return false;
                    }

                    if (obj == null)
                    {
                        return false;
                    }

                    return true;
                }
                catch (Exception)
                {
                    obj = null;
                    return false;
                }
            }

            public void Setup(Assembly assembly)
            {
                var customBeatmapDataType = assembly.GetType("CustomJSONData.CustomBeatmap.CustomBeatmapData");

                _cjdLevelCustomDataType = customBeatmapDataType?.GetProperty("levelCustomData", BindingFlags.Public | BindingFlags.Instance);

                _isValid = _cjdLevelCustomDataType != null;
            }
        }
    }
}