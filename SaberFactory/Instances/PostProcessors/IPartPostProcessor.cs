using UnityEngine;

namespace SaberFactory.Instances.PostProcessors
{
    public interface IPartPostProcessor
    {
        void ProcessPart(GameObject partObject);
    }
}