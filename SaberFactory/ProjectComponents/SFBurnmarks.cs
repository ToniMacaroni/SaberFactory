using UnityEngine;

namespace SaberFactory.ProjectComponents
{
    public class SFBurnmarks : MonoBehaviour
    {
        [Header("Parameters")]
        public float RandomBurnmarkJitter = 0.001f;

        public float FadeStrength = 0.3f;

        public float BurnmarkSize = 0.1f;

        [Help("Only Assign the materials you actually need")]
        [Header("Basic Materials")]
        [Tooltip("Material for the spark particle system")]
        public Material SparkleMaterial;

        [Tooltip("Material for the linerenderer of the burnmarks")]
        public Material BurnMarkMaterial;

        [Tooltip("The rendertexture of the burnmarks will be passed to this material relative to the floor")]
        public Material FloorMaterial;

        [Header("Advanced Materials")]
        [Tooltip("Material to use for blitting the faded rendertexture")]
        public Material FadeoutMaterial;
    }
}