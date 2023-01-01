using System.Collections.Generic;
using System.Linq;
using CustomSaber;
using SaberFactory.Instances;
using SaberFactory.Models;
using UnityEngine;

namespace SaberFactory.Helpers;

internal static class SaberHelpers
{
    /// <summary>
    /// Gets the native trails of a CustomSaber where the transforms are valid
    /// and orders them by z position.
    /// </summary>
    /// <param name="saberObject">The CustomSaber <see cref="GameObject"/> to look on (includes chilren).</param>
    /// <returns></returns>
    public static List<CustomTrail> GetTrails(GameObject saberObject)
    {
        if (!saberObject)
        {
            return null;
        }

        return saberObject
            .GetComponentsInChildren<CustomTrail>()
            .Where(x => x.PointEnd != null && x.PointStart != null)
            .OrderByDescending(x => x.PointEnd.position.z)
            .ToList();
    }

    /// <summary>
    /// Searches for the SaberMonoBehaviour in the parents.
    /// </summary>
    /// <param name="go">Child <see cref="GameObject"/></param>
    /// <returns>The SaberMonoBehaviour if found. Null if not</returns>
    public static SaberInstance.SaberMonoBehaviour GetSaberMonoBehaviour(GameObject go)
    {
        return go.GetComponentInParent<SaberInstance.SaberMonoBehaviour>();
    }

    /// <summary>
    /// Savely transforms source <see cref="TrailModel"/> into dest <see cref="TrailModel"/>
    /// Used mainly for using the trail of another saber.
    /// Creates a destModel if it doesn't exist.
    /// Transforms in place and also returns the result.
    /// </summary>
    /// <param name="destModel">Transform destination</param>
    /// <param name="srcModel">Transform source</param>
    /// <returns></returns>
    public static TrailModel TransformInto(this TrailModel destModel, TrailModel srcModel)
    {
        if (destModel == null)
        {
            destModel = new TrailModel();
            destModel.CopyFrom(srcModel);
            destModel.TrailOriginTrails = srcModel.TrailOriginTrails;
            destModel.Material.UpdateBackupMaterial(false); // set backup material to current material

            return destModel;
        }

        destModel.CopyFrom(srcModel);
        destModel.TrailOriginTrails = srcModel.TrailOriginTrails;
        return destModel;
    }
}