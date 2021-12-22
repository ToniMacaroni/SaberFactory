using System.Diagnostics;
using SiraUtil.Logging;
using SiraUtil.Tools;
using Debug = UnityEngine.Debug;

namespace SaberFactory.Helpers
{
    public class DebugTimer
    {
        private readonly Stopwatch _stopwatch;
        private readonly string _taskName;

        public DebugTimer(string taskName = null)
        {
            _taskName = taskName ?? "Task";
            _stopwatch = new Stopwatch();
        }

        public static DebugTimer StartNew(string taskName = null)
        {
            var sw = new DebugTimer(taskName);
            sw.Start();
            return sw;
        }

        public void Start()
        {
            _stopwatch.Start();
        }

        public void Print()
        {
            Debug.LogError(GetString());
        }

        public void Print(SiraLog logger)
        {
            logger.Info(GetString());
        }

        private string GetString()
        {
            _stopwatch.Stop();
            return $"{_taskName} finished in {_stopwatch.Elapsed.Seconds}.{_stopwatch.Elapsed.Milliseconds}s";
        }
    }
}