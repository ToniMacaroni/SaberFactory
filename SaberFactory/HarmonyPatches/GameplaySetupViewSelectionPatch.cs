using System.Collections.Generic;
using HarmonyLib;
using HMUI;
using IPA.Utilities;

namespace SaberFactory.HarmonyPatches
{
    [HarmonyPatch(typeof(GameplaySetupViewController), "SetActivePanel")]
    public class GameplaySetupViewSelectionPatch
    {
        public static bool Prefix(int panelIdx, int ____activePanelIdx, TextSegmentedControl ____selectionSegmentedControl)
        {
            if (!GameplaySetupViewPatch.EntryEnabled)
            {
                return true;
            }

            if (panelIdx == GameplaySetupViewPatch.SaberPanelIdx)
            {
                var cell =
                    ____selectionSegmentedControl.GetField<List<SegmentedControlCell>, SegmentedControl>("_cells")[
                        GameplaySetupViewPatch.SaberPanelIdx];

                ____selectionSegmentedControl.SelectCellWithNumber(____activePanelIdx);
                Editor.LegacyEditor.Instance?.Open();
                cell.ClearHighlight(SelectableCell.TransitionType.Instant);
                return false;
            }

            return true;
        }
    }
}