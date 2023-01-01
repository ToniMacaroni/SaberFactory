using HarmonyLib;
using HMUI;

namespace SaberFactory.HarmonyPatches
{
    // If state of flow coordinators changes always close saber factory

    [HarmonyPatch(typeof(FlowCoordinator), "Activate")]
    public class ActivateFlowCoordinatorPatch
    {
        public static void Prefix()
        {
            Editor.LegacyEditor.Instance?.Close(true);
        }
    }
}