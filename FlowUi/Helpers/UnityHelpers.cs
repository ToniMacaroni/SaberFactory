using System.Diagnostics;
using System.Text;
using HarmonyLib;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;


namespace FlowUi.Helpers
{
    public static class UnityHelpers
    {
        private delegate Object GetObjectByIdDel(int instanceId);
        private static readonly GetObjectByIdDel GetObjectByIdImpl;

        private delegate bool DoesObjectWithIdExistDel(int instanceId);
        private static readonly DoesObjectWithIdExistDel DoesObjectWithIdExistImpl;

        public static Dictionary<string, Object> ObjectMap = new();

        static UnityHelpers()
        {
            GetObjectByIdImpl = (GetObjectByIdDel)AccessTools.Method(typeof(Object), "FindObjectFromInstanceID", new[] { typeof(int) }).CreateDelegate(typeof(GetObjectByIdDel));
            DoesObjectWithIdExistImpl = (DoesObjectWithIdExistDel)AccessTools.Method(typeof(Object), "DoesObjectWithInstanceIDExist", new[] { typeof(int) }).CreateDelegate(typeof(DoesObjectWithIdExistDel));
        }

        public static bool TryGetObjectById<T>(int instanceId, out T val) where T : Object
        {
            val = GetObjectByIdImpl(instanceId) as T;
            return (bool)val;
        }

        public static void MapObjects()
        {
            if (GetObjectByIdImpl != null && DoesObjectWithIdExistImpl != null)
            {
                Debug.Log("Found Functions");
            }
            
            var str = new StringBuilder();
            var sw = new Stopwatch();
            sw.Start();
            var found = 0;
            for (var i = 0; i < 70000; i++)
            {
                var obj = GetObjectByIdImpl(i);
                if (obj)
                {
                    str.AppendLine($"{i}: {obj.name}");
                    //found++;
                    // ObjectMap[obj.name] = obj;
                    found++;
                }
            }
            
            sw.Stop();
            Debug.Log($"Mapped in {sw.ElapsedMilliseconds}");
            
            Debug.Log($"Mapped {found} objects");
            File.WriteAllText("ObjectMap.txt", str.ToString());
        }
    }
}