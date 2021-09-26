using UnityEngine;

namespace SaberFactory.Instances.Middleware
{
    public interface ISaberMiddleware
    {
        void ProcessSaber(GameObject saberObject);
    }
}