using System.Collections.Generic;
using UnityEngine;

namespace SaberFactory.Helpers
{
    internal class ShaderPropertyCache
    {
        public ShaderPropertyInfo this[Shader shader] => Get(shader);

        private readonly Dictionary<string, ShaderPropertyInfo> _shaderPropertyInfos =
            new Dictionary<string, ShaderPropertyInfo>();

        public ShaderPropertyInfo Get(Shader shader)
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
}