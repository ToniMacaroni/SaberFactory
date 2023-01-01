namespace SaberFactory.Helpers
{
    public static class BeatmapHelper
    {
        public static float GetLastNoteTime(this BeatmapData beatmapData)
        {
            var lastTime = 0f;

            foreach (var noteData in beatmapData.GetBeatmapDataItems<NoteData>(0))
            {
                if (noteData.colorType == ColorType.None)
                {
                    continue;
                }

                if (noteData.time > lastTime)
                {
                    lastTime = noteData.time;
                }
            }
        
            return lastTime;
        }
    }
}