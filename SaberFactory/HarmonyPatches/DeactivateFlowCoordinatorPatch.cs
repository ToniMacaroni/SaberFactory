using HarmonyLib;
using HMUI;

namespace SaberFactory.HarmonyPatches
{
    [HarmonyPatch(typeof(FlowCoordinator), "Deactivate")]
    public class DeactivateFlowCoordinatorPatch
    {
        public static void Prefix()
        {
            Editor.Editor.Instance?.Close(true);
        }
    }
}