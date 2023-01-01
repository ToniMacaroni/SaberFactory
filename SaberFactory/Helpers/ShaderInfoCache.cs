using System.Collections.Generic;
using UnityEngine;

namespace SaberFactory.Helpers;

/// <summary>
/// Static cache for shader infos.
/// </summary>
internal static class ShaderInfoCache
{
    private static readonly Dictionary<string, ShaderPropertyInfo> _shaderPropertyInfos = new();

    /// <summary>
    /// Gets the <see cref="ShaderPropertyInfo"/> for the specified shader.
    /// Creates and caches one if it doesn't exist.
    /// </summary>
    /// <param name="shader">The shader to get the <see cref="ShaderPropertyInfo"/> of</param>
    /// <returns></returns>
    public static ShaderPropertyInfo Get(Shader shader)
    {
        if (_shaderPropertyInfos.TryGetValue(shader.name, out var info))
        {
            return info;
        }

        info = new ShaderPropertyInfo(shader);
        _shaderPropertyInfos.Add(shader.name, info);
        return info;
    }
}