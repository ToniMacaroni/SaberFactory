using UnityEngine;

namespace SaberFactory.Instances.PostProcessors
{
    public interface ISaberPostProcessor
    {
        void ProcessSaber(GameObject saberObject);
    }
}