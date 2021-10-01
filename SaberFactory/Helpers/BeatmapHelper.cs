namespace SaberFactory.Helpers
{
    public static class BeatmapHelper
    {
        public static float GetLastNoteTime(this BeatmapData beatmapData)
        {
            var lastTime = 0f;
            var beatmapLinesData = beatmapData.beatmapLinesData;
            foreach (BeatmapLineData beatMapLineData in beatmapLinesData)
            {
                var beatmapObjectsData = beatMapLineData.beatmapObjectsData;
                for (var i = beatmapObjectsData.Count - 1; i >= 0; i--)
                {
                    var beatmapObjectData = beatmapObjectsData[i];
                    if (beatmapObjectData.beatmapObjectType == BeatmapObjectType.Note && ((NoteData)beatmapObjectData).colorType != ColorType.None)
                        if (beatmapObjectData.time > lastTime)
                            lastTime = beatmapObjectData.time;
                }
            }

            return lastTime;
        }
    }
}