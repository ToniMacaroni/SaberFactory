using System;
using FlowUi.Runtime;
using UnityEngine;

namespace SaberFactory.UI.Flow;

public partial class SaberCell
{
    private IAssetInfo _asset;

    public override void SetData(object data)
    {
        _asset = (IAssetInfo)data;

        var n = _asset.Name;
        

        nameTextmesh.text = Sanitize(_asset.Name);
        authorTextmesh.text = Sanitize(_asset.Author);
        coverImage.sprite = _asset.Cover;
        coverImage.enabled = _asset.Cover;

        favoriteImage.color = _asset.IsFavorite ? new Color(0.875f, 0.086f, 0.435f) : new Color(1,1,1,0.1f);
    }
    
    private string Sanitize(string str)
    {
        if(str.Length > 32)
        {
            return str.Substring(0, 32) + "...";
        }

        return str;
    }
}