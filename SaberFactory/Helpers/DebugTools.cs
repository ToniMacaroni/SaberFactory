using UnityEngine;

namespace SaberFactory.Helpers
{
    public static class DebugTools
    {
        public static void LogError(object message)
        {
            Debug.LogError(message);
        }
    }
}