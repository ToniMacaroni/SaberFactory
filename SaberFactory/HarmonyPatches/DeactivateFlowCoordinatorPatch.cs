using HarmonyLib;
using HMUI;

namespace SaberFactory.HarmonyPatches
{
    [HarmonyPatch(typeof(FlowCoordinator), "Deactivate")]
    public class DeactivateFlowCoordinatorPatch
    {
        public static void Prefix()
        {
            Editor.LegacyEditor.Instance?.Close(true);
        }
    }
}