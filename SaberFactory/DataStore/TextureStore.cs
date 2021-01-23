using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SaberFactory.Helpers;

namespace SaberFactory.DataStore
{
    internal class TextureStore
    {
        private readonly Dictionary<string, TextureAsset> _textureAssets;

        private TextureStore()
        {
            _textureAssets = new Dictionary<string, TextureAsset>();
        }

        public Task<TextureAsset> this[string path] => GetTexture(path);

        public async Task<TextureAsset> GetTexture(string path)
        {
            if (!HasTexture(path))
            {
                return await LoadTexture(path);
            }

            return _textureAssets[path];
        }

        public bool HasTexture(string path)
        {
            return _textureAssets.ContainsKey(path);
        }

        public IEnumerable<TextureAsset> GetAllTextures()
        {
            return _textureAssets.Values;
        }

        private async Task<TextureAsset> LoadTexture(string path)
        {
            var fullPath = PathTools.ToFullPath(path);
            if (!File.Exists(fullPath)) return null;

            var tex = await Readers.ReadTexture(path);
            if (!tex) return null;

            var texAsset = new TextureAsset(Path.GetFileName(path), path, tex, EAssetOrigin.FileSystem);

            _textureAssets.Add(texAsset.Path, texAsset);
            return texAsset;
        }
    }
}