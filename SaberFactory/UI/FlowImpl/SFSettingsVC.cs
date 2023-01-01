using System;
using System.Threading.Tasks;
using UnityEngine;

namespace SaberFactory.UI.Flow;

public partial class SFSettingsVC
{
    protected override async Task Setup()
    {
        navigationSegmentedControl.SetTexts(Enum.GetNames(typeof(ESettingsCategory)));
        await SelectCategory((int)ESettingsCategory.Main);
    }

    protected override Task DidOpen()
    {
        navigationSegmentedControl.didSelectCellEvent += CategoryCellSelected;
        return base.DidOpen();
    }

    protected override Task DidClose()
    {
        navigationSegmentedControl.didSelectCellEvent -= CategoryCellSelected;
        return base.DidClose();
    }
}